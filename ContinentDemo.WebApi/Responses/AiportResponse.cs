namespace ContinentDemo.WebApi.Responses
{
    //using Location;

    //public class Rootobject
    //{
    //    public string iata { get; set; }
    //    public string name { get; set; }
    //    public string city { get; set; }
    //    public string city_iata { get; set; }
    //    public string icao { get; set; }
    //    public string country { get; set; }
    //    public string country_iata { get; set; }
    //    public Location location { get; set; }
    //    public int rating { get; set; }
    //    public int hubs { get; set; }
    //    public string timezone_region_name { get; set; }
    //    public string type { get; set; }
    //}
    
    public class AirportResponse
    {
        public string? iata { get; set; }
        public LocationResponse? location { get; set; }
    }

    public class LocationResponse
    {
        public double lon { get; set; }
        public double lat { get; set; }
    }
}
