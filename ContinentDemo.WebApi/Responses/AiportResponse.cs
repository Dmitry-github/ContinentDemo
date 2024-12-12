namespace ContinentDemo.WebApi.Responses
{
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
