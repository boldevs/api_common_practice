using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using StackExchange.Redis; // Add this for Redis-specific exceptions
using Microsoft.Extensions.Logging; // Add this for logging

namespace Product.API.Caching
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _cache;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IDistributedCache cache, ILogger<RedisCacheService> logger)
        {
            _cache = cache;
            _logger = logger;
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            try
            {
                var jsonData = await _cache.GetStringAsync(key);
                return jsonData is null ? default : JsonSerializer.Deserialize<T>(jsonData);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis while getting key: {Key}", key);
                return default; // Return default value if Redis is unavailable
            }
        }

        public async Task SetDataAsync<T>(string key, T value, TimeSpan? absoluteExpireTime = null, TimeSpan? unusedExpireTime = null)
        {
            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = absoluteExpireTime ?? TimeSpan.FromSeconds(60),
                    SlidingExpiration = unusedExpireTime
                };

                var jsonData = JsonSerializer.Serialize(value);
                await _cache.SetStringAsync(key, jsonData, options);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis while setting key: {Key}", key);
            }
        }

        public async Task RemoveDataAsync(string key)
        {
            try
            {
                await _cache.RemoveAsync(key);
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, "Redis configuration is invalid: {Message}", ex.Message);
            }
            catch (RedisConnectionException ex)
            {
                _logger.LogError(ex, "Failed to connect to Redis while removing key: {Key}", key);
            }
        }
    }
}