using System.Net.Http.Json;
namespace PWA.Auth.Services;

public class WorldBetApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    
    public WorldBetApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }
    
    public async Task<T?> GetFromJsonAsync<T>(string endpoint)
    {
        var client = _httpClientFactory.CreateClient("WorldBetAPI");
        return await client.GetFromJsonAsync<T>(endpoint);
    }
    
    public async Task<HttpResponseMessage> PostAsJsonAsync<T>(string endpoint, T data)
    {
        var client = _httpClientFactory.CreateClient("WorldBetAPI");
        return await client.PostAsJsonAsync(endpoint, data);
    }
    
    public async Task<HttpResponseMessage> PutAsJsonAsync<T>(string endpoint, T data)
    {
        var client = _httpClientFactory.CreateClient("WorldBetAPI");
        return await client.PutAsJsonAsync(endpoint, data);
    }
    
    public async Task<HttpResponseMessage> DeleteAsync(string endpoint)
    {
        var client = _httpClientFactory.CreateClient("WorldBetAPI");
        return await client.DeleteAsync(endpoint);
    }
}