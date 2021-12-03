using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Core.Models;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Inventory.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Factories
{
    public sealed class VehicleFactory : ISingleton
    {
        public string Name => "VehicleFactory";

        private readonly IServiceProvider _serviceProvider;
        private readonly InventoryFactory _inventoryFactory;

        public VehicleFactory(IServiceProvider serviceProvider, InventoryFactory inventoryFactory)
        {
            _serviceProvider = serviceProvider;
            _inventoryFactory = inventoryFactory;

            RAGE.Entities.Vehicles.CreateEntity = netHandle => Create(netHandle);
        }

        public SVehicle Create(NetHandle netHandle)
        {
            try
            {
                var player = ActivatorUtilities.CreateInstance<SVehicle>(_serviceProvider, netHandle);
                if (player is null)
                    Console.WriteLine("Unable to create vehicle.");

                return player!;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null!;
            }
        }

        public async void Load(SVehicle vehicle, SPlayer player, Database.Entities.Vehicle dbVehicle)
        {
            vehicle.Id = dbVehicle.Id;
            vehicle.OwnerId = dbVehicle.OwnerId;
            vehicle.Team = dbVehicle.Team;
            vehicle.Dimension = player.Dimension;

            vehicle.Inventory = await _inventoryFactory.Load(InventoryTypes.VEHICLE, dbVehicle.Id);
        }
    }
}
