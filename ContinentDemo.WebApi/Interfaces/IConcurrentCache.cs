namespace ContinentDemo.WebApi.Interfaces
{
    public interface IConcurrentCache<TKey, TValue> where TKey : notnull
    {
        public bool Store(TKey key, TValue value, TimeSpan expiresAfter);
        public TValue? Get(TKey key);
    }
}