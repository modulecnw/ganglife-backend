using GTANetworkAPI;
using Sibaui.Core.Extensions;
using Sibaui.Game.Client.Data.Message;
using Sibaui.Game.Module.Gangwar.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using static Sibaui.Game.Factories.Entities.SPlayer;

// made by Dschahannam.
namespace Sibaui.Game.Factories.Entities
{
    public class SMarker : Marker
    {
        public SMarker(NetHandle handle) : base(handle)
        {

        }

        public async Task<Vector3> GetPosition() => await NAPI.Task.RunReturn(() => Position);

        public void CreateMessage(string text, int duration, NotificationType type)
        {
            var colShape = (SColShape)NAPI.ColShape.CreateSphereColShape(Position, Scale, Dimension);
            colShape.Message = new Message(text, duration, type);
        }

        public void CreateMessage(string text, string title, int duration, NotificationType type)
        {
            var colShape = (SColShape)NAPI.ColShape.CreateSphereColShape(Position, Scale, Dimension);
            colShape.Message = new Message(text, title, duration, type);
        }
    }
}
