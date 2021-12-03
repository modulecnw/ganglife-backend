using Sibaui.Game.Client.Data.Garage;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Module.Garage.Interface
{
    public interface IModuleGarage
    {
        string Name { get; }

        void GetAvailableGarageVehicles(ref List<GarageVehicleDataModel> vehicles);
    }
}
