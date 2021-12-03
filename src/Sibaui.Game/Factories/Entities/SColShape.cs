using GTANetworkAPI;
using Sibaui.Core.Extensions;
using Sibaui.Game.Client.Data.Message;
using Sibaui.Game.Module.Gangwar.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

// made by Dschahannam.
namespace Sibaui.Game.Factories.Entities
{
    public enum ColShapeTypes
    {
        NONE,
        GANGWAR
    }

    public class SColShape : ColShape
    {
        public new int Id { get; set; }
        public GangwarInfoModel GangwarInfo { get; set; }
        public GangwarFlagModel GangwarFlag { get; set; }
        public Message Message { get; set; }
        public Dictionary<int, SPlayer> Players { get; set; }

        public SColShape(NetHandle handle) : base(handle)
        {
            Id = -1;
            GangwarFlag = null;
            Players = new Dictionary<int, SPlayer>();
        }

        public async Task<Vector3> GetPosition() => await NAPI.Task.RunReturn(() => Position);
    }
}
