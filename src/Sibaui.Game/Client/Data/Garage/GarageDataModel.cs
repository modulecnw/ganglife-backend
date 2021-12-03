using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Garage
{
    public class GarageDataModel
    {
        public int I { get; set; }
        public string N { get; set; }
        public List<GarageVehicleDataModel> Data { get; set; }

        public GarageDataModel(int Id, string Name, List<GarageVehicleDataModel> Data)
        {
            I = Id;
            N = Name;
            this.Data = Data;
        }
    }
}
