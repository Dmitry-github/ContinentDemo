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

        public async Task<double?> GetDistanceFromCacheAsync(string key)
        {
            var cashed = await _cache.GetStringAsync(key);
            
            return double.TryParse(cashed, out var result) ? result: null;
        }

        public async Task StoreDistanceToCacheAsync(string key, double value)
        {
            await _cache.SetStringAsync(key, value.ToString(CultureInfo.CurrentCulture), _options);
        }

        public async Task<Location?> GetLocationFromCacheAsync(string key)
        {
            var cashedString = await _cache.GetStringAsync(key);

            return Location.FromString(cashedString);
        }

        public async Task StoreLocationToCacheAsync(string key, Location value)
        {
            await _cache.SetStringAsync(key, value.ToString(), _options);
        }
    }
}
