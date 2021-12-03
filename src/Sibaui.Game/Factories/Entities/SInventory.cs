using GTANetworkAPI;
using Sibaui.Core.Extensions;
using Sibaui.Database.Entities;
using Sibaui.Game.Client.Data.Inventory;
using Sibaui.Game.Client.Data.Message;
using Sibaui.Game.Module.Gangwar.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Factories.Entities
{
    public class SInventory
    {
        public int Id { get; set; }
        public bool Locked { get; set; }
        public int TargetId { get; set; }
        public InventoryType InventoryType { get; set; }
        public Dictionary<int, InventoryItem> Items { get; set; } // Key = Slot | Value = Item

        public SInventory()
        {
            Id = -1;
            InventoryType = null;
            Locked = false;
            Items = new Dictionary<int, InventoryItem>();
        }

        public SingleInventoryModel GetSingleInventoryModel() => new SingleInventoryModel(this);
    }
}
