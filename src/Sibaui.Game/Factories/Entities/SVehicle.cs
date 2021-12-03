using GTANetworkAPI;
using Sibaui.Core.Extensions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Factories.Entities
{
    public class SVehicle : Vehicle
    {
        public new int Id { get; set; }
        public SInventory Inventory { get; set; }
        public new bool Locked
        {
            get => NAPI.Vehicle.GetVehicleLocked(this);
            set
            {
                SetSharedData("Locked", value);
                NAPI.Vehicle.SetVehicleLocked(this, value);
            }
        }
        public new bool EngineStatus
        {
            get => NAPI.Vehicle.GetVehicleEngineStatus(this);
            set
            {
                SetSharedData("Engine", value);
                NAPI.Vehicle.SetVehicleEngineStatus(this, value);
            }
        }

        public string Name { get => $"({Id}) - {DisplayName}"; }

        public int OwnerId { get; set; }
        public Database.Entities.Team Team { get; set; }

        public SVehicle(NetHandle handle) : base(handle)
        {
            Id = -1;
            Inventory = null;
            Locked = true;
            EngineStatus = false;
            OwnerId = -1;
            Team = null;
        }

        public async Task<Vector3> GetPosition()
        {
            await NAPI.Task.WaitForMainThread(0);
            return Position;
        }
    }
}
