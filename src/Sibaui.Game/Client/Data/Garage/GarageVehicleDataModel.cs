using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Client.Data.Garage
{
    public class GarageVehicleDataModel
    {
        public int I { get; set; }
        public string N { get; set; }
        public string No { get; set; }

        public GarageVehicleDataModel(int Id, string Name, string Notice)
        {
            I = Id;
            N = Name;
            No = Notice;
        }

        public GarageVehicleDataModel(int Id, string Name)
        {
            I = Id;
            N = Name;
            No = String.Empty;
        }
    }
}
