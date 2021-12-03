using GTANetworkAPI;
using Sibaui.Game.Factories.Entities;
using Sibaui.Game.Models;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Module.Gangwar.Model
{
    public class GangwarInfoModel
    {
        public int AvailableRewards { get; set; }
        public Database.Entities.Gangwar Gangwar { get; set; }
        public GangwarGameModel Game { get; set; }
        public Blip Blip { get; set; }
        public SMarkerShape Territory { get; set; }
        public SMarkerShape Flag1 { get; set; }
        public SMarkerShape Flag2 { get; set; }
        public SMarkerShape Flag3 { get; set; }

        public SMarkerShape GetFlagByHandle(NetHandle handle)
        {
            if (Flag1.Shape.Handle == handle)
                return Flag1;

            if (Flag2.Shape.Handle == handle)
                return Flag2;

            if (Flag3.Shape.Handle == handle)
                return Flag3;

            return null;
        }
    }
}
