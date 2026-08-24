using System.Text.Json;
using ApiDashboard.DTOs;
using Microsoft.Extensions.Caching.Distributed;

namespace ApiDashboard.Services;

public class IrongridRedisService
{
    private readonly IDistributedCache _cache;

    public IrongridRedisService(IDistributedCache cache)
    {
        _cache = cache;
    }

    public async Task<AssetsStatusDto> GetAsync(int id)
    {
        var keyName = $"assets-status:{id}";

        var cachedData = await _cache.GetStringAsync(keyName);
        if (cachedData == null)
        {
            return null;
        }

        var obj = JsonSerializer.Deserialize<AssetsStatusDto>(cachedData);

        return obj;
    }

    public async Task SaveAsync<T>(int id, T value)
    {
        var keyName = $"assets-status:{id}";

        var jsonObj = JsonSerializer.Serialize(value);

        await _cache.SetStringAsync(keyName, jsonObj);
    }
}