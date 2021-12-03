using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class Team
    {
        [NotMapped]
        public Vector3 SpawnPosition => new Vector3(SpawnPositionX, SpawnPositionY, SpawnPositionZ);

        [NotMapped]
        public Vector3 ManagePosition => new Vector3(ManagePositionX, ManagePositionY, ManagePositionZ);

        [NotMapped]
        public string HexColor => string.Format("#{0:X2}{1:X2}{2:X2}", ColorR, ColorG, ColorB);
    }
}
