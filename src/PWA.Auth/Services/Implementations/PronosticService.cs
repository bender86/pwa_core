using System.Net.Http.Json;
using dto.worldbet.Models;
using dto.worldbet.Requests;

namespace PWA.Auth.Services;

public class PronosticService : IPronosticService
{
    private readonly WorldBetApiClient _apiClient;

    public PronosticService(WorldBetApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<PronosticDto> CreateOrUpdatePronosticAsync(CreatePronosticRequest request)
    {
        var response = await _apiClient.PostAsJsonAsync("pronostic", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PronosticDto>() ?? throw new Exception();
    }

    public async Task<List<PronosticDto>> GetMyPronosticsAsync()
    {
        return await _apiClient.GetFromJsonAsync<List<PronosticDto>>(
            "pronostic/my-pronostics") ?? new();
    }

    public async Task<PronosticDto?> GetMyPronosticForMatchAsync(int matchId)
    {
        return await _apiClient.GetFromJsonAsync<PronosticDto>(
            $"pronostic/my-pronostics/match/{matchId}");
    }

    public async Task<MatchPronosticsDto> GetMatchPronosticsAsync(int matchId)
    {
        return await _apiClient.GetFromJsonAsync<MatchPronosticsDto>(
            $"pronostic/match/{matchId}") ?? throw new Exception();
    }

    public async Task DeletePronosticAsync(int pronosticId)
    {
        var response = await _apiClient.DeleteAsync($"pronostic/{pronosticId}");
        response.EnsureSuccessStatusCode();
    }
}