using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using NetTopologySuite.Geometries;

namespace LGED.Core.Utils
{
    public class MyPoint : Point
    {
        //     [JsonConstructor]
        // public MyPoint(double latitude, double longitude, int srid)
        //     :base(new GeoAPI.Geometries.Coordinate(longitude, latitude))
        // {
        //     SRID = srid;
        // }
        public MyPoint(CoordinateSequence coordinates, GeometryFactory factory) : base(coordinates, factory)
        {
        }
    }
}