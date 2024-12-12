namespace ContinentDemo.WebApi.Caching
{
    using Interfaces;
    using Location;
    
    public class LocalCacheStorage: ICacheStorage
    {
        //private static ConcurrentCache<string, double?> _cache = null!; 
        private static IConcurrentCache<string, Location?> _cache = null!;
        private readonly int _localCacheExpiresAfterHours;

        public LocalCacheStorage()
        {
            //_cache = new ConcurrentCache<string, double?>();
            _cache = new ConcurrentCache<string, Location?>();
            _localCacheExpiresAfterHours = ConfigAppSettings.LocalCacheExpiresAfterHours;
        }

        /*
        public async Task<double?> GetDistanceFromCacheAsync(string key)
        {
            //return _cache.Get(key);
            return await Task.Run(() => _cache.Get(key));
        }

        public async Task StoreDistanceToCacheAsync(string key, double value)
        {
            _cache.Store(key, value, TimeSpan.FromHours(ConfigAppSettings.LocalCacheExpiresAfterHours));

            await Task.Run(() => _cache.Store(key, value, TimeSpan.FromHours(ConfigAppSettings.LocalCacheExpiresAfterHours)));
        }
        */

        public async Task<Location?> GetLocationFromCacheAsync(string key)
        {
            return await Task.Run(() => _cache.Get(key));
        }

        public async Task StoreLocationToCacheAsync(string key, Location value)
        {
            await Task.Run(() => _cache.Store(key, value, TimeSpan.FromHours(_localCacheExpiresAfterHours)));
        }
    }
}
