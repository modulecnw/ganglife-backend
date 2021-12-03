using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class Player
    {
        [NotMapped]
        public Vector3 Position
        {
            get => new Vector3(PositionX, PositionY, PositionZ);
            set => (PositionX, PositionY, PositionZ) = (value.X, value.Y, value.Z);
        }
    }
}
