using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class Garage
    {
        [NotMapped]
        public Vector3 Position => new Vector3(PositionX, PositionY, PositionZ);

        [NotMapped]
        public Vector3 SpawnPosition => new Vector3(SpawnPositionX, SpawnPositionY, SpawnPositionZ);
    }
}
