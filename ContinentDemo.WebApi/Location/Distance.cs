namespace ContinentDemo.WebApi.Location
{
    public class Distance
    {
        private const double MidEarthRadius = 6371d;
        private const double MileInKm = 1.852d;

        //public static double GetDistance(Location location1, Location location2)
        //{
        //    var point1 = new Point(new Coordinate(location1.Latitude, location1.Longitude));
        //    var point2 = new Point(new Coordinate(location2.Latitude, location2.Longitude));

        //    var distanceInMeters = point1.Distance(point2);

        //    return distanceInMeters;
        //}


        //public double findDistance(Location location1, Location location2)
        //{
        //    //var ctfac = new CoordinateT;

        //    ////var from = GeographicCoordinateSystem.WGS84;
        //    ////var to = ProjectedCoordinateSystem.WebMercator;

        //    var SourceCoordSystem = new  CreateFromCoordinateSystems(CalculateDistance())
        //    var TargetCoordSystem = new CoordinateSystemFactory().CreateFromWkt(ToWKT);

        //    var trans = new CoordinateTransformationFactory().CreateFromCoordinateSystems(SourceCoordSystem, TargetCoordSystem);

        //    var geom = new LineString()
        //    var projGeom = Transform(geom, trans.MathTransform);

        //    //return projGeom;

        //    //var trans = ctfac.CreateFromCoordinateSystems(from, to);
        //    var mathTransform = trans.MathTransform;

        //    var p1Coordinate = new Coordinate(location1.Latitude, location1.Longitude);
        //    var p2Coordinate = new Coordinate(location2.Latitude, location2.Longitude);

        //    p1Coordinate = mathTransform.Transform(p1Coordinate);
        //    p2Coordinate = mathTransform.Transform(p2Coordinate);

        //    return p1Coordinate.Distance(p2Coordinate);
        //}

        //static NetTopologySuite.Geometries.Geometry Transform(NetTopologySuite.Geometries.Geometry geom, MathTransform transform)
        //{
        //    geom = geom.Copy();
        //    geom.Apply(new MTF(transform));
        //    return geom;
        //}

        //public double CalculateDistance(Location point1, Location point2)
        //{
        //    var d1 = point1.Latitude * (Math.PI / 180.0);
        //    var num1 = point1.Longitude * (Math.PI / 180.0);
        //    var d2 = point2.Latitude * (Math.PI / 180.0);
        //    var num2 = point2.Longitude * (Math.PI / 180.0) - num1;
            
        //    var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) +
        //             Math.Cos(d1) * Math.Cos(d2) * Math.Pow(Math.Sin(num2 / 2.0), 2.0);
            
        //    return MidEarthRadius * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        //}


        public static double GetDistance(Location location1, Location location2)
        {
            return GetDistanceFromLatLonInKm(location1.Longitude, location1.Latitude, location2.Longitude, location2.Latitude);
        }

        public static double GetDistanceInMiles(Location location1, Location location2)
        {
            return Math.Round(Km2Miles(GetDistance(location1, location2)), 3);
        }

        private static double GetDistanceFromLatLonInKm(double lon1, double lat1, double lon2, double lat2)
        {
            var dLat = Deg2Rad(lat2 - lat1);
            var dLon = Deg2Rad(lon2 - lon1);
            var a =
                Math.Sin(dLat / 2d) * Math.Sin(dLat / 2d) +
                Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) *
                Math.Sin(dLon / 2d) * Math.Sin(dLon / 2d);
            var c = 2d * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1d - a));
            var d = MidEarthRadius * c;
            return d;
        }

        private static double Deg2Rad(double deg)
        {
            return deg * (Math.PI / 180d);
        }

        private static double Km2Miles(double km)
        {
            return km / MileInKm;
        }
    }

    //public static Geometry ProjectTo(this Geometry geometry, int srid)
    //{
    //    var transformation = _coordinateSystemServices.CreateTransformation(geometry.SRID, srid);

    //    var result = geometry.Copy();
    //    result.Apply(new MathTransformFilter(transformation.MathTransform));

    //    return result;
    //}
}
