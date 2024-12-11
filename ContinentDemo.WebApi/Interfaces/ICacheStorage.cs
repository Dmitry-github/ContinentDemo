namespace ContinentDemo.WebApi.Interfaces
{
    using Location;

    public interface ICacheStorage
    {
        //Task<double?> GetDistanceFromCacheAsync(string key);
        //Task StoreDistanceToCacheAsync(string key, double value);
        Task<Location?> GetLocationFromCacheAsync(string key);
        Task StoreLocationToCacheAsync(string key, Location value);
    }
}