namespace ContinentDemo.WebApi.Caching
{
    using System.Collections.Concurrent;

    public class CacheItem<T>
    {
        public CacheItem(T value, TimeSpan expiresAfter)
        {
            Value = value;
            ExpiresAfter = expiresAfter;
        }
        public T Value { get; }
        internal DateTimeOffset Created { get; } = DateTimeOffset.Now;
        internal TimeSpan ExpiresAfter { get; }
    }


    public class ConcurrentCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _initialCapacity = ConfigAppSettings.LocalCacheInitialCapacity;
        private readonly int _numProcs = Environment.ProcessorCount;
        //private int concurrencyLevel = _numProcs * 2;

        //private readonly ConcurrentDictionary<TKey, CacheItem<TValue?>> _cache = new ConcurrentDictionary<TKey, CacheItem<TValue?>>();
        private readonly ConcurrentDictionary<TKey, CacheItem<TValue?>> _cache;
        private readonly ILogger<ConcurrentCache<TKey, TValue>> _logger;

        public ConcurrentCache()
        {
            int concurrencyLevel = _numProcs * 2;
            _cache = new ConcurrentDictionary<TKey, CacheItem<TValue?>>(concurrencyLevel, _initialCapacity);

            //_logger = logger;
            //_logger = new Logger<ConcurrentCache<TKey, TValue>>(new LoggerFactory());

            _logger = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace).AddConsole()).CreateLogger<ConcurrentCache<TKey, TValue>>();
        }

        public bool Store(TKey key, TValue value, TimeSpan expiresAfter)
        {
            //_cache[key] = new CacheItem<TValue>(value, expiresAfter);

            var added = _cache.TryAdd(key, new CacheItem<TValue?>(value, expiresAfter));

            _logger.Log(LogLevel.Information, $"{(added ? "Added to cache" : "Cannot Add")} : {key} - {value} ...");

            return added;
        }

        public TValue? Get(TKey key)
        {
            if (!_cache.ContainsKey(key)) return default(TValue);
            var cached = _cache[key];

            if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
            {
                if (!_cache.TryRemove(key, out var notRemoved))
                    _logger.Log(LogLevel.Warning, $"Cannot remove expired: {key} - {notRemoved} ...");

                return default(TValue);
            }
            return cached.Value;
        }
    }
}
