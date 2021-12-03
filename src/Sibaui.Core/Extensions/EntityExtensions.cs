using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.Text;

// made by Dschahannam.
namespace Sibaui.Core.Extensions
{
    public static class EntityExtensions
    {
        public static Tuple<ColShape, Marker> CreateMarkerColshape(this GTANetworkMethods.Entity _, MarkerType markerType, Vector3 Position, float Scale, uint Dimension = UInt32.MaxValue)
        {
            Color color = new Color(57, 192, 216, 200);

            var colShape = NAPI.ColShape.CreateCylinderColShape(Position, Scale, Scale, Dimension);
            var marker = NAPI.Marker.CreateMarker(markerType, Position, new Vector3(), new Vector3(), Scale, color, false, Dimension);

            return new Tuple<ColShape, Marker>(colShape, marker);
        }
    }
}
