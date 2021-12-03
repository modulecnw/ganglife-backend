using GTANetworkAPI;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Module.Inventory.Models
{
    public class LocalInventoryData
    {
        public LocalInventoryData(SInventory playerInventory, SInventory moduleInventory)
        {
            PlayerInventory = playerInventory;
            ModuleInventory = moduleInventory;

            Inventories = new List<SInventory>() { PlayerInventory, ModuleInventory };
        }

        public SInventory PlayerInventory { get; set; }
        public SInventory ModuleInventory { get; set; }
        public List<SInventory> Inventories { get; set; }

        public async Task RunTransaction(SPlayer player, Func<Task> transaction)
        {
            if (PlayerInventory.Locked)
            {
                player.SendNotification("Dein Inventar ist derzeit gesperrt. Bitte versuche deine Interaktion erneut.", NotificationType.ERROR, "No-Duping Policy");
                return;
            }

            if (ModuleInventory != null && ModuleInventory.Locked)
            {
                player.SendNotification("Das zweite Inventar ist derzeit gesperrt. Bitte versuche deine Interaktion erneut.", NotificationType.ERROR, "No-Duping Policy");
                return;
            }

            PlayerInventory.Locked = true;
            if (ModuleInventory != null) ModuleInventory.Locked = true;

            await transaction();

            PlayerInventory.Locked = false;
            if (ModuleInventory != null) ModuleInventory.Locked = false;

            await player.PlayAnimation("mp_safehousevagos@", "package_dropoff", 9, 1500);
        }
    }
}
