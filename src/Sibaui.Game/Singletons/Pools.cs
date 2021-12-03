using GTANetworkAPI;
using Sibaui.Core.Models;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Singletons
{
    public sealed class Pools : ISingleton
    {
        public string Name => "Pools";

        public IEnumerable<SPlayer> GetAllPlayers()
        {
            return NAPI.Pools.GetAllPlayers().Cast<SPlayer>();
        }

        public IEnumerable<SVehicle> GetAllVehicles()
        {
            return NAPI.Pools.GetAllVehicles().Cast<SVehicle>();
        }
    }
}
