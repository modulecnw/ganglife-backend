using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sibaui.Core.Enumerations;
using Sibaui.Core.Extensions;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Client.Data.Garage;
using Sibaui.Game.Events;
using Sibaui.Game.Factories;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Gangwar;
using Sibaui.Game.Module.Garage.Interface;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Module.Garage
{
    public sealed class GarageModule : IModule
    {
        public string Name => "Garage";

        private readonly ILogger<GarageModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly IServiceProvider _serviceProvider;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;
        private readonly SContext _sContext;

        private readonly VehicleFactory _vehicleFactory;
        private GangwarModule _gangwarModule;

        public static Dictionary<int, Database.Entities.Garage> Garages;

        public GarageModule(ILogger<GarageModule> logger, IServiceScopeFactory serviceScopeFactory, IServiceProvider serviceProvider, EventHub eventHub, Pools pools, SContext sContext,
                            VehicleFactory vehicleFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _serviceProvider = serviceProvider;
            _eventHub = eventHub;
            _pools = pools;
            _sContext = sContext;

            _vehicleFactory = vehicleFactory;

            Garages = new Dictionary<int, Database.Entities.Garage>();
        }

        public Task StartAsync()
        {
            _gangwarModule = _serviceProvider.GetModule<GangwarModule>();

            _eventHub.OnKeyPress += OnKeyPress;

            _sContext.Garages.ForEach(Load);
            _sContext.Vehicles.ForEach(ParkAll);

            _sContext.SaveChanges();

            NAPI.ClientEvent.Register<SPlayer, int>("GetInparkVehicles", this, GetInparkVehicles);
            NAPI.ClientEvent.Register<SPlayer, int, int>("ParkIn", this, ParkIn);
            NAPI.ClientEvent.Register<SPlayer, int, int>("ParkOut", this, ParkOut);

            return Task.CompletedTask;
        }

        private void ParkAll(Database.Entities.Vehicle vehicle) => vehicle.Parked = true;

        private void Load(Database.Entities.Garage garage)
        {
            Garages.Add(garage.Id, garage);

            if (garage.TeamId <= 1)
                NAPI.Blip.CreateBlip(50, garage.Position, 1, 30, $"Garage {garage.Name}");
        }

        public Task StopAsync()
        {
            _eventHub.OnKeyPress -= OnKeyPress;
            Garages.Clear();

            return Task.CompletedTask;
        }

        private async Task OnKeyPress(SPlayer player, Keys key)
        {
            if (key != Keys.KEY_E) return;

            Vector3 playerPos = await player.GetPosition();

            Database.Entities.Garage dbGarage = Garages.FirstOrDefault(g => g.Value.Position.DistanceTo(playerPos) <= 3).Value;
            if (dbGarage == null) return;

            if (dbGarage.TeamId > 1)
                if (dbGarage.TeamId != player.TeamInfo.Team.Id)
                {
                    player.SendNotification("Diese Garage gehört nicht zu deiner Fraktion.", NotificationType.ERROR);
                    return;
                }

            using var scope = _serviceScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<SContext>();

            List<GarageVehicleDataModel> vehicles = new List<GarageVehicleDataModel>();

            // Module-Garages
            IModuleGarage garage;

            // Gangwar:
            if (player.DimensionType == DimensionTypes.GANGWAR)
            {
                garage = (IModuleGarage)_gangwarModule;
                garage.GetAvailableGarageVehicles(ref vehicles);

                _logger.LogInformation($"GetAvailableGarageVehicles Gangwar < {await player.GetName()}");
            }
            else await db.Vehicles.Where(v => v.OwnerId == player.Id && v.Parked).ForEach((v) => vehicles.Add(new GarageVehicleDataModel(v.Id, v.Model)));

            player.TriggerEventSafe("ShowIF", "Garage", new GarageDataModel(dbGarage.Id, dbGarage.Name, vehicles).ToJson());

            await Task.CompletedTask;
        }

        private void GetInparkVehicles(SPlayer player, int garageId)
        {
            if (player.DimensionType != DimensionTypes.WORLD) return;

            _logger.LogInformation($"GetInparkVehicles < {player.Name} - {garageId}");

            if (Garages.TryGetValue(garageId, out Database.Entities.Garage dbGarage))
            {
                List<GarageVehicleDataModel> vehicles = new List<GarageVehicleDataModel>();
                _pools.GetAllVehicles().Where(v => v.Position.DistanceTo(dbGarage.Position) <= 10 && v.OwnerId == player.Id).ForEach((v) => vehicles.Add(new GarageVehicleDataModel(v.Id, v.DisplayName)));

                player.TriggerEvent("UpdateView", "SendInparkVehicles", new GarageDataModel(garageId, "", vehicles).ToJson());
            }
        }

        private async void ParkIn(SPlayer player, int vehicleId, int garageId)
        {
            if (Garages.TryGetValue(garageId, out Database.Entities.Garage dbGarage))
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<SContext>();

                Database.Entities.Vehicle dbVehicle = await db.Vehicles.FindAsync(vehicleId);
                if (dbVehicle == null) return;
                if (dbVehicle.Parked) return;

                SVehicle vehicle = _pools.GetAllVehicles().FirstOrDefault((v) => v.Id == dbVehicle.Id);
                if (vehicle == null) return;

                NAPI.Task.Run(() =>
                {
                    vehicle.Delete();

                    player.SendNotification("Fahrzeug eingeparkt.", NotificationType.SUCCESS, "Garage");
                });

                dbVehicle.Parked = true;
                dbVehicle.GarageId = garageId;

                await db.SaveChangesAsync();
            }
        }

        private async void ParkOut(SPlayer player, int vehicleId, int garageId)
        {

            if (Garages.TryGetValue(garageId, out Database.Entities.Garage dbGarage))
            {
                Database.Entities.Vehicle dbVehicle = null;

                // Module-Garages
                IModuleGarage garage;

                // Gangwar:
                if (player.DimensionType == DimensionTypes.GANGWAR)
                {
                    List<GarageVehicleDataModel> vehicles = new List<GarageVehicleDataModel>();

                    garage = (IModuleGarage)_gangwarModule;
                    garage.GetAvailableGarageVehicles(ref vehicles);

                    var targetVehicle = vehicles.FirstOrDefault(v => v.I == vehicleId);
                    if (targetVehicle == null) return;

                    dbVehicle = new Database.Entities.Vehicle()
                    {
                        Id = targetVehicle.I,
                        Model = targetVehicle.N,
                        OwnerId = player.Id,
                        Team = player.TeamInfo.Team,
                        GarageId = garageId
                    };

                    _logger.LogInformation($"Parkout Gangwar < {await player.GetName()} - {garageId}");
                }
                else
                {
                    using var scope = _serviceScopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<SContext>();

                    dbVehicle = await db.Vehicles.Include(t => t.Team).FirstOrDefaultAsync(v => v.Id == vehicleId);
                    if (dbVehicle == null) return;
                    if (!dbVehicle.Parked) return;

                    SVehicle vehicle = _pools.GetAllVehicles().FirstOrDefault(v => v.Id == dbVehicle.Id);
                    if (vehicle != null) return;

                    dbVehicle.Parked = false;
                    dbVehicle.GarageId = garageId;

                    await db.SaveChangesAsync();
                }

                NAPI.Task.Run(() =>
                {
                    SVehicle vehicle = (SVehicle)NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(dbVehicle.Model), dbGarage.SpawnPosition, dbGarage.SpawnPositionHeading, 0, 0);
                    _vehicleFactory.Load(vehicle, player, dbVehicle);

                    player.SendNotification("Fahrzeug ausgeparkt.", NotificationType.SUCCESS, "Garage");
                });
            }

        }
    }
}
