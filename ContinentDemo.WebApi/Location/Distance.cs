namespace ContinentDemo.WebApi.Location
{
    public class Distance
    {
        private const double MidEarthRadius = 6371d;
        private const double MileInKm = 1.852d;

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
}
