
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using ProductAPI.Services.Caching;

namespace ProductAPI.Services.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache? _cache;

        public RedisCacheService(IDistributedCache? cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var data = await _cache!.GetStringAsync(key);
            if (data is null)
            {
                return default(T);
            }
            try
            {
                return JsonSerializer.Deserialize<T>(data, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
            }
            catch (JsonException ex)
            {
                Console.WriteLine($"Deserialization error: {ex.Message}");
                return default;
            }
        }

        public async void RemoveData(string key)
        {
            _cache!.Remove("products");
        }

        public async void SetData<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5)
            };

            _cache?.SetString(key, JsonSerializer.Serialize(data), options);
        }
    }
}