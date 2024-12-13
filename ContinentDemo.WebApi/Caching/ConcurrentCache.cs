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
        private readonly ConcurrentDictionary<TKey, CacheItem<TValue?>> _cache;

        public ConcurrentCache()
        {
            int concurrencyLevel = _numProcs * 2;
            _cache = new ConcurrentDictionary<TKey, CacheItem<TValue?>>(concurrencyLevel, _initialCapacity);
        }

        public bool Store(TKey key, TValue value, TimeSpan expiresAfter)
        {
            var added = _cache.TryAdd(key, new CacheItem<TValue?>(value, expiresAfter));

            return added;
        }

        public TValue? Get(TKey key)
        {
            if (!_cache.ContainsKey(key)) return default(TValue);
            var cached = _cache[key];

            if (DateTimeOffset.Now - cached.Created >= cached.ExpiresAfter)
            {
                _cache.TryRemove(key, out var notRemoved);
            }

            return cached.Value;
        }
    }
}
