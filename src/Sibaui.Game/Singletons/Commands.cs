using GTANetworkAPI;
using Microsoft.Extensions.DependencyInjection;
using Sibaui.Core.Extensions;
using Sibaui.Core.Models;
using Sibaui.Database.Context;
using Sibaui.Database.Entities;
using Sibaui.Game.Factories;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Module.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Singletons
{
    public sealed class Commands : Script
    {
        [Command("pos")]
        public void pos(SPlayer player, string name = "")
        {
            //GOOD CODE
            Console.WriteLine(name + " | " + player.Position.X.ToString().Replace(",", ".") + ", " + player.Position.Y.ToString().Replace(",", ".") + ", " + player.Position.Z.ToString().Replace(",", ".") + " - " + player.Rotation.Z.ToString().Replace(", ", "."));
        }

        [Command("veh")]
        public void veh(SPlayer player, string hash, int owner = 1)
        {
            SVehicle vehicle = (SVehicle)NAPI.Vehicle.CreateVehicle(NAPI.Util.GetHashKey(hash), player.Position, player.Rotation.Z, 0, 0);
            vehicle.Id = 1;
            vehicle.OwnerId = owner;
        }

        [Command("stop")]
        public void stop(SPlayer player)
        {
            IServiceProvider serviceProvider = Resource.Instance._serviceProvider;
            foreach (var service in serviceProvider.GetServices<IModule>())
            {
                Console.WriteLine($"Stopping {service.Name}");
                service.StopAsync();
            }
        }

        [Command("start")]
        public void start(SPlayer player)
        {
            IServiceProvider serviceProvider = Resource.Instance._serviceProvider;
            foreach (var service in serviceProvider.GetServices<IModule>())
            {
                Console.WriteLine($"Starting {service.Name}");
                service.StartAsync();
            }
        }

        [Command("additem")]
        public async void additem(SPlayer player, string itemName, int amount = 1)
        {
            IServiceProvider serviceProvider = Resource.Instance._serviceProvider;
            var inventoryModule = serviceProvider.GetModule<InventoryModule>();
            var inventoryFactory = serviceProvider.GetService<InventoryFactory>();

            Item targetItem = inventoryModule.Items.FirstOrDefault(item => item.Value.Name.ToLower().StartsWith(itemName.ToLower())).Value;
            if(targetItem == null)
            {
                player.SendNotification($"Item {itemName} nicht gefunden.", NotificationType.ERROR);
                return;
            }

            if (player.Inventory == null)
            {
                player.SendNotification($"Dein Inventar ist ungültig.", NotificationType.ERROR);
                return;
            }

            await inventoryFactory.AddItem(player.Inventory, targetItem, amount);
        }
    }
}
