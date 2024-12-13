namespace ContinentDemo.WebApi.Caching
{
    using Interfaces;
    using Location;
    
    public class LocalCacheStorage: ICacheStorage
    {
        private static IConcurrentCache<string, Location?> _cache = null!;
        private readonly int _localCacheExpiresAfterHours;

        public LocalCacheStorage()
        {
            _cache = new ConcurrentCache<string, Location?>();
            _localCacheExpiresAfterHours = ConfigAppSettings.LocalCacheExpiresAfterHours;
        }

        public Task<Location?> GetLocationFromCacheAsync(string key)
        {
            return Task.FromResult(_cache.Get(key));
        }

        public Task<bool> StoreLocationToCacheAsync(string key, Location value)
        {
            var added = _cache.Store(key, value, TimeSpan.FromHours(_localCacheExpiresAfterHours));
            return Task.FromResult(added);
        }
    }
}
