using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Inventory
{
    public class ItemDataModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("itemId")]
        public int ItemId { get; set; }

        [JsonProperty("amount")]
        public int Amount { get; set; }

        public ItemDataModel(int id, int itemId, int amount)
        {
            Id = id;
            ItemId = itemId;
            Amount = amount;
        }
    }
}
