using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Team
{
    public class TeamPlayerDataModel
    {
        [JsonProperty("i")]
        public int Id { get; set; }

        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("r")]
        public int Rank { get; set; }

        [JsonProperty("b")]
        public bool Bank { get; set; }

        [JsonProperty("v")]
        public bool Manage { get; set; }

        [JsonProperty("m", NullValueHandling = NullValueHandling.Ignore)]
        public int? Magazines { get; set; }

        [JsonProperty("l")]
        public DateTime LastSeen { get; set; }

        [JsonProperty("o")]
        public bool Online { get; set; }

        public TeamPlayerDataModel(int id, string name, int rank, bool bank, bool manage, int? magazines, DateTime lastSeen, bool online)
        {
            Id = id;
            Name = name;
            Rank = rank;
            Bank = bank;
            Manage = manage;
            Magazines = magazines;
            LastSeen = lastSeen;
            Online = online;
        }
    }
}
