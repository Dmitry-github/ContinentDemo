namespace ContinentDemo.WebApi.Interfaces
{
    using Location;

    public interface ILocationLogic
    {
        public Task<Location?> GetLocationFromRequestAsync(string iata);
        public Task<(double?, string)> GetDistanceAsync(string iata1, string iata2);
        public Task<Location?> GetLocationFromCacheAsync(string iata);
        public Task StoreLocationToCacheAsync(string iata, Location location);
        public Task<Location?> GetLocationAsync(string iata);
    }
}