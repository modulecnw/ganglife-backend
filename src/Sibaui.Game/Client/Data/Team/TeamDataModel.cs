using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Team
{
    public class TeamDataModel
    {
        [JsonProperty("n")]
        public string Name { get; set; }

        [JsonProperty("data")]
        public List<TeamPlayerDataModel> Data { get; set; }

        [JsonProperty("v")]
        public bool Manage { get; set; }

        [JsonProperty("r")]
        public int Rank { get; set; }

        public TeamDataModel(string name, List<TeamPlayerDataModel> data, bool manage, int rank)
        {
            Name = name;
            Data = data;
            Manage = manage;
            Rank = rank;
        }
    }
}
