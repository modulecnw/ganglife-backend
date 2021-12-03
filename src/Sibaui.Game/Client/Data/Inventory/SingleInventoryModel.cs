using Newtonsoft.Json;
using Sibaui.Database.Entities;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Inventory
{
    public class SingleInventoryModel
    {
        [JsonProperty("i")]
        public int Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("w")]
        public int Weight { get; set; }

        [JsonProperty("s")]
        public List<ItemDataModel> Items { get; set; }

        public SingleInventoryModel(SInventory inventory)
        {
            Id = inventory.Id;
            Name = inventory.InventoryType.DisplayName;
            Weight = inventory.InventoryType.MaxWeight;
            Items = new List<ItemDataModel>();

            for (int i = 0; i < inventory.InventoryType.MaxSlots; i++)
            {
                if (inventory.Items.TryGetValue(i, out InventoryItem item))
                    Items.Add(new ItemDataModel(i, item.ItemId, item.Amount));
                else
                    Items.Add(new ItemDataModel(i, 0, 0));
            }
        }
    }
}
