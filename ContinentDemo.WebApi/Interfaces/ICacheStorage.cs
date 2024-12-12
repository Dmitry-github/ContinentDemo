namespace ContinentDemo.WebApi.Interfaces
{
    using Location;

    public interface ICacheStorage
    {
        Task<Location?> GetLocationFromCacheAsync(string key);
        Task StoreLocationToCacheAsync(string key, Location value);
    }
}