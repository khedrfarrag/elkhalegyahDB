using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Alkhaligya.BLL.Services.Cashing
{
    public class CacheService : ICacheService
    {
        private readonly IMemoryCache _memoryCache;

        public CacheService(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public async Task<T> GetAsync<T>(string key)
        {
            _memoryCache.TryGetValue(key, out T value);
            return await Task.FromResult(value);
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan expiration)
        {
            _memoryCache.Set(key, value, expiration);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(string key)
        {
            _memoryCache.Remove(key);
            await Task.CompletedTask;
        }
    }

}
