namespace ContinentDemo.WebApi.Interfaces
{
    using Location;

    public interface ICacheStorage
    {
        Task<Location?> GetLocationFromCacheAsync(string key);
        Task<bool> StoreLocationToCacheAsync(string key, Location value);
    }
}