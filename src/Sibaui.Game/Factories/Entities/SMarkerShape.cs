using GTANetworkAPI;
using Sibaui.Game.Factories.Entities;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Game.Models
{
    public class SMarkerShape
    {
        public SMarkerShape(ColShape shape, Marker marker)
        {
            Shape = (SColShape)shape;
            Marker = (SMarker)marker;
        }

        public SMarkerShape(Tuple<ColShape, Marker> colMarker)
        {
            Shape = (SColShape)colMarker.Item1;
            Marker = (SMarker)colMarker.Item2;
        }

        public SColShape Shape { get; set; }
        public SMarker Marker { get; set; }
    }
}
