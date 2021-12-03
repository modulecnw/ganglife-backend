using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Database.Entities
{
    public partial class Gangwar
    {
        [NotMapped]
        public Vector3 MiddlePosition => new Vector3(MiddlePositionX, MiddlePositionY, MiddlePositionZ);

        [NotMapped]
        public Vector3 AttackPosition => new Vector3(AttackPositionX, AttackPositionY, AttackPositionZ);

        [NotMapped]
        public Vector3 RewardPosition => new Vector3(RewardPositionX, RewardPositionY, RewardPositionZ);

        [NotMapped]
        public Vector3 Flag1Position => new Vector3(Flag1PositionX, Flag1PositionY, Flag1PositionZ);

        [NotMapped]
        public Vector3 Flag2Position => new Vector3(Flag2PositionX, Flag2PositionY, Flag2PositionZ);

        [NotMapped]
        public Vector3 Flag3Position => new Vector3(Flag3PositionX, Flag3PositionY, Flag3PositionZ);
    }
}
