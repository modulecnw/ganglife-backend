using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Team.Hideout
{
    public class TeamHideoutDataModel
    {
        [JsonProperty("m")]
        public int OverallAmount { get; set; }

        [JsonProperty("k")]
        public int OwnAmount { get; set; }

        [JsonProperty("g", NullValueHandling = NullValueHandling.Ignore)]
        public string GangwarName { get; set; }

        [JsonProperty("l", NullValueHandling = NullValueHandling.Ignore)]
        public bool Lead { get; set; }

        public TeamHideoutDataModel(int overallAmount, int ownAmount)
        {
            OverallAmount = overallAmount;
            OwnAmount = ownAmount;
            GangwarName = null;
            Lead = false;
        }

        public TeamHideoutDataModel(int overallAmount, int ownAmount, string gangwarName)
        {
            OverallAmount = overallAmount;
            OwnAmount = ownAmount;
            GangwarName = gangwarName;
            Lead = false;
        }

        public TeamHideoutDataModel(int overallAmount, int ownAmount, string gangwarName, bool lead)
        {
            OverallAmount = overallAmount;
            OwnAmount = ownAmount;
            GangwarName = gangwarName;
            Lead = lead;
        }


        public TeamHideoutDataModel(int overallAmount, int ownAmount, bool lead)
        {
            OverallAmount = overallAmount;
            OwnAmount = ownAmount;
            GangwarName = null;
            Lead = true;
        }
    }
}
