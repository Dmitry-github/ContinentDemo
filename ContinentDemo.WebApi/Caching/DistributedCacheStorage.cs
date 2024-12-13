namespace ContinentDemo.WebApi.Caching
{
    using Interfaces;
    using Location;
    using System.Globalization;
    using Microsoft.Extensions.Caching.Distributed;

    public class DistributedCacheStorage : ICacheStorage
    {
        private readonly IDistributedCache _cache;
        private readonly DistributedCacheEntryOptions _options;

        public DistributedCacheStorage(IDistributedCache distributedCache)
        {
            _cache = distributedCache;
            _options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(ConfigAppSettings.RedisAbsoluteExpirationHours),
                SlidingExpiration = TimeSpan.FromHours(ConfigAppSettings.RedisSlidingExpirationHours)
            };
        }

        public async Task<Location?> GetLocationFromCacheAsync(string key)
        {
            var cashedString = await _cache.GetStringAsync(key);

            return Location.FromString(cashedString);
        }

        public async Task<bool> StoreLocationToCacheAsync(string key, Location value)
        {
            await _cache.SetStringAsync(key, value.ToString(), _options);
            return true;
        }
    }
}
