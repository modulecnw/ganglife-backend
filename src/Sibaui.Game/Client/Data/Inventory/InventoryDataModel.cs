using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Inventory
{
    public class InventoryDataModel
    {
        [JsonProperty("i")]
        public List<SingleInventoryModel> Inventories { get; set; }

        public InventoryDataModel(SingleInventoryModel playerInventory, SingleInventoryModel otherInventory)
        {
            Inventories = new List<SingleInventoryModel>() { playerInventory, otherInventory };
        }

        public InventoryDataModel(SingleInventoryModel playerInventory)
        {
            Inventories = new List<SingleInventoryModel>() { playerInventory };
        }
    }
}
