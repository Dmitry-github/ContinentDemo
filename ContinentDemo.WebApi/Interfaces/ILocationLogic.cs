namespace ContinentDemo.WebApi.Interfaces
{
    using Location;

    public interface ILocationLogic
    {
        public Task<Location?> GetLocationFromRequestAsync(string iata);
        //public Task<double> GetDistanceFromIatasAsync(string iata1, string iata2);
        public Task<double?> GetDistanceFromCacheAsync(string iata1, string iata2);
        public Task StoreDistanceToCacheAsync(string iata1, string iata2, double distance);
    }
}