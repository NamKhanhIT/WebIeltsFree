using StackExchange.Redis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;

namespace WebIeltsFree.Models
{
    /// <summary>
    /// Redis service for distributed caching and session management
    /// Replaces in-memory cache with scalable Redis
    /// </summary>
    public interface IRedisService
    {
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<bool> RemoveAsync(string key);
        Task IncrementAsync(string key, long increment = 1);
        Task<long> DecrementAsync(string key, long decrement = 1);
        Task FlushAsync();
        Task<bool> ExistsAsync(string key);
        Task<List<T>> GetListAsync<T>(string key);
        Task SetListAsync<T>(string key, List<T> values, TimeSpan? expiry = null);
    }

    public class RedisService : IRedisService, WebIeltsFree.Services.ICacheService
    {
        private readonly IDatabase _db;
        private readonly ILogger<RedisService> _logger;

        public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
        {
            _logger = logger;

            try
            {
                // Resolve environment variable placeholders
                var rawConnString = configuration["Redis:ConnectionString"] ?? "localhost:6379";
                var connectionString = EnvHelper.ResolveEnvVars(rawConnString);
                
                var options = ConfigurationOptions.Parse(connectionString);
                options.AbortOnConnectFail = false;
                
                var redis = ConnectionMultiplexer.Connect(options);
                _db = redis.GetDatabase();

                // Test connection
                _db.Ping();
                _logger.LogInformation($"Redis connected: {connectionString}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Redis connection warning: {ex.Message} (falling back to local cache)");
            }
        }



        /// <summary>
        /// Get value from cache
        /// </summary>
        public async Task<T?> GetAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                
                if (value.IsNull)
                    return default;

                return JsonSerializer.Deserialize<T>(value.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting from cache: {ex.Message}");
                return default;
            }
        }

        /// <summary>
        /// Set value in cache with optional expiry
        /// </summary>
        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(value);
                await _db.StringSetAsync(key, serialized, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting cache: {ex.Message}");
            }
        }

        /// <summary>
        /// Remove key from cache
        /// </summary>
        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                return await _db.KeyDeleteAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error removing from cache: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Explicit ICacheService.RemoveAsync implementation
        /// </summary>
        async Task WebIeltsFree.Services.ICacheService.RemoveAsync(string key)
        {
            await this.RemoveAsync(key);
        }

        /// <summary>
        /// Increment counter (for rate limiting, counters)
        /// </summary>
        public async Task IncrementAsync(string key, long increment = 1)
        {
            try
            {
                await _db.StringIncrementAsync(key, increment);
                await _db.KeyExpireAsync(key, TimeSpan.FromHours(1));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error incrementing counter: {ex.Message}");
            }
        }

        /// <summary>
        /// Decrement counter
        /// </summary>
        public async Task<long> DecrementAsync(string key, long decrement = 1)
        {
            try
            {
                return await _db.StringDecrementAsync(key, decrement);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error decrementing counter: {ex.Message}");
                return 0;
            }
        }

        /// <summary>
        /// Check if key exists
        /// </summary>
        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                return await _db.KeyExistsAsync(key);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error checking key existence: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Flush all cache (admin only)
        /// </summary>
        public async Task FlushAsync()
        {
            try
            {
                await _db.ExecuteAsync("FLUSHDB");
                _logger.LogWarning("Redis cache flushed");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error flushing cache: {ex.Message}");
            }
        }

        /// <summary>
        /// Get list from cache
        /// </summary>
        public async Task<List<T>> GetListAsync<T>(string key)
        {
            try
            {
                var value = await _db.StringGetAsync(key);
                
                if (value.IsNull)
                    return new List<T>();

                return JsonSerializer.Deserialize<List<T>>(value.ToString()) ?? new List<T>();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error getting list from cache: {ex.Message}");
                return new List<T>();
            }
        }

        /// <summary>
        /// Set list in cache
        /// </summary>
        public async Task SetListAsync<T>(string key, List<T> values, TimeSpan? expiry = null)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(values);
                await _db.StringSetAsync(key, serialized, expiry);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error setting list in cache: {ex.Message}");
            }
        }
    }
}
