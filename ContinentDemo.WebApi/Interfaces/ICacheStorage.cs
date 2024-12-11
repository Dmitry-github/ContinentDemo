namespace ContinentDemo.WebApi.Interfaces
{
    public interface ICacheStorage
    {
        Task<double?> GetFromCacheAsync(string key);
        Task StoreToCacheAsync(string key, double value);
    }
}