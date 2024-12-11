namespace ContinentDemo.WebApi.Caching
{
    using Interfaces;
    
    public class LocalCacheStorage: ICacheStorage
    {
        private static ConcurrentCache<string, double?> _cache = null!; 

        public LocalCacheStorage()
        {
            _cache = new ConcurrentCache<string, double?>();
        }
        
        public async Task<double?> GetFromCacheAsync(string key)
        {
            //return _cache.Get(key);
            return await Task.Run(() => _cache.Get(key));
        }

        public async Task StoreToCacheAsync(string key, double value)
        {
            //_cache.Store(key, value, TimeSpan.FromHours(ConfigAppSettings.LocalCacheExpiresAfterHours));

            await Task.Run(() => _cache.Store(key, value, TimeSpan.FromHours(ConfigAppSettings.LocalCacheExpiresAfterHours)));
        }
    }
}
