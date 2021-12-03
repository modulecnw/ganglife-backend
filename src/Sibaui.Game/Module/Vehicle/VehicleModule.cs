using GTANetworkAPI;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Sibaui.Core.Enumerations;
using Sibaui.Core.Extensions;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Client.Data.Char;
using Sibaui.Game.Client.Data.Garage;
using Sibaui.Game.Client.Data.Team;
using Sibaui.Game.Events;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Inventory.Enumerations;
using Sibaui.Game.Module.Inventory.Interface;
using Sibaui.Game.Module.Login.Extensions;
using Sibaui.Game.Singletons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Module.Gangwar
{

    public sealed class VehicleModule : IModule, IModuleInventory
    {
        public string Name => "VehicleModule";

        private readonly ILogger<VehicleModule> _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly EventHub _eventHub;
        private readonly Pools _pools;

        public VehicleModule(ILogger<VehicleModule> logger, IServiceScopeFactory serviceScopeFactory, EventHub eventHub, Pools pools)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            _eventHub = eventHub;
            _pools = pools;
        }

        public Task StartAsync()
        {
            NAPI.ClientEvent.Register<SPlayer, SVehicle>("ToggleDoor", this, ToggleDoor);
            NAPI.ClientEvent.Register<SPlayer, SVehicle>("ToggleEngine", this, ToggleEngine);

            _eventHub.OnPlayerEnterVehicle += OnPlayerEnterVehicle;

            return Task.CompletedTask;
        }

        public Task StopAsync() => Task.CompletedTask;

        private Task OnPlayerEnterVehicle(SPlayer player, SVehicle vehicle, sbyte seatID)
        {
            if (vehicle == null) return Task.CompletedTask;
            player.TriggerEventSafe("VehEnter", 50, 0, vehicle.EngineStatus);

            return Task.CompletedTask;
        }

        private void ToggleDoor(SPlayer player, SVehicle vehicle)
        {
            player.SendNotification(vehicle.Locked ? "Fahrzeug aufgeschlossen." : "Fahrzeug zugeschlossen.", vehicle.Locked ? NotificationType.SUCCESS : NotificationType.ERROR, vehicle.Name);
            vehicle.Locked = !vehicle.Locked;
        }

        private void ToggleEngine(SPlayer player, SVehicle vehicle)
        {
            player.SendNotification(vehicle.EngineStatus ? "Motor gestoppt." : "Motor gestartet.", vehicle.EngineStatus ? NotificationType.ERROR : NotificationType.SUCCESS, vehicle.Name);
            vehicle.EngineStatus = !vehicle.EngineStatus;
        }

        public Task<SInventory> GetModuleInventory(SPlayer player)
        {
            SVehicle vehicle = _pools.GetAllVehicles().ToList().FirstOrDefault(v => v.Position.DistanceTo(player.Position) < 2.75f);
            if (vehicle == null) return Task.FromResult<SInventory>(null);

            return Task.FromResult(vehicle.Inventory);
        }

        public Task<bool?> CanAccessModuleInventory(SPlayer player, SInventory inventory)
        {
            // Gilt derzeit für alle Fahrzeuge ohne Standart KG Angabe - ergo: Rework muss demnächst her.
            if (inventory.InventoryType.Id != (int)InventoryTypes.VEHICLE) return Task.FromResult<bool?>(null);

            SVehicle vehicle = _pools.GetAllVehicles().ToList().FirstOrDefault(v => v.Id == inventory.TargetId);
            if (vehicle == null) return Task.FromResult<bool?>(false);

            if (vehicle.Position.DistanceTo(player.Position) > 3.5f)
                return Task.FromResult<bool?>(false);

            return Task.FromResult<bool?>(true);
        }
    }
}
