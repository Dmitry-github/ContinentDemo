namespace ContinentDemo.WebApi.Caching
{
    using Interfaces;
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


    public class ConcurrentCache<TKey, TValue> : IConcurrentCache<TKey, TValue> where TKey : notnull
    {
        private readonly int _initialCapacity = ConfigAppSettings.LocalCacheInitialCapacity;

        private readonly int _numProcs = Environment.ProcessorCount;
        //private int concurrencyLevel = _numProcs * 2;

        //private readonly ConcurrentDictionary<TKey, CacheItem<TValue?>> _cache = new ConcurrentDictionary<TKey, CacheItem<TValue?>>();
        private readonly ConcurrentDictionary<TKey, CacheItem<TValue?>> _cache;
        private readonly ILogger<ConcurrentCache<TKey, TValue>> _logger;
        private readonly bool _extendedLog;

        public ConcurrentCache()
        {
            int concurrencyLevel = _numProcs * 2;
            _cache = new ConcurrentDictionary<TKey, CacheItem<TValue?>>(concurrencyLevel, _initialCapacity);

            //_logger = logger;
            //_logger = new Logger<ConcurrentCache<TKey, TValue>>(new LoggerFactory());

            _logger = LoggerFactory.Create(loggingBuilder => loggingBuilder
                .SetMinimumLevel(LogLevel.Trace).AddConsole()).CreateLogger<ConcurrentCache<TKey, TValue>>();
            _extendedLog = ConfigAppSettings.ExtendedLogEnabled;
        }

        public bool Store(TKey key, TValue value, TimeSpan expiresAfter)
        {
            //_cache[key] = new CacheItem<TValue>(value, expiresAfter);

            var added = _cache.TryAdd(key, new CacheItem<TValue?>(value, expiresAfter));

            if (_extendedLog)
                _logger.Log(LogLevel.Information, $"{(added ? "Added to cache" : "Cannot Add")} : {key} - {value?.ToString()}");

            return added;
        }

        public TValue? Get(TKey key)
        {
            if (!_cache.ContainsKey(key)) return default(TValue);
            var cached = _cache[key];
            
            if (_extendedLog)
                _logger.Log(LogLevel.Information, $"Get from cache: {key} - {cached.ToString()}");

            if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
            {
                var removed = _cache.TryRemove(key, out var notRemoved);

                if (_extendedLog)
                    _logger.Log(LogLevel.Warning, $"{(removed ? "Removed" : "Cannot remove")} expired: {key} - {notRemoved?.ToString()}");

                //return default(TValue);
            }

            return cached.Value;
        }
    }
}
