using BookStore.Application.Interfaces;

namespace BookStore.Infrastructure.Services;

/// <summary>
/// A simple in-memory cache service using Dictionary.
/// This implementation does NOT use IMemoryCache as per requirements.
/// </summary>
public class DictionaryBasedCacheService : ICacheService
{
    private readonly Dictionary<string, CacheEntry> _cache = new();
    private readonly object _lock = new();

    private class CacheEntry
    {
        public object? Value { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public T? Get<T>(string key)
    {
        lock (_lock)
        {
            if (!_cache.TryGetValue(key, out var entry))
                return default;

            // Check if expired
            if (DateTime.UtcNow > entry.ExpiresAt)
            {
                _cache.Remove(key);
                return default;
            }

            return (T?)entry.Value;
        }
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        lock (_lock)
        {
            _cache[key] = new CacheEntry
            {
                Value = value,
                ExpiresAt = DateTime.UtcNow.Add(expiration)
            };
        }
    }

    public void Remove(string key)
    {
        lock (_lock)
        {
            _cache.Remove(key);
        }
    }
}
