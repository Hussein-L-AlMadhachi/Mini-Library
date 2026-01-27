using BookStore.Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace BookStore.Infrastructure.Services;

public class MemoryCacheService(IMemoryCache memoryCache) : ICacheService
{
    public T? Get<T>(string key)
    {
        // TryGetValue is efficient/safe
        memoryCache.TryGetValue(key, out T? value);
        return value;
    }

    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        var options = new MemoryCacheEntryOptions
        {
            // INTERNAL DETAIL: 
            // AbsoluteExpiration: Cache dies after X minutes strictly.
            // SlidingExpiration: Cache stays alive if people keep accessing it.
            // We use Absolute here for simplicity and safety.
            AbsoluteExpirationRelativeToNow = expiration 
        };

        memoryCache.Set(key, value, options);
    }

    public void Remove(string key)
    {
        memoryCache.Remove(key);
    }
}