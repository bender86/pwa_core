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

    public async Task<List<PronosticDto>> GetMyPronosticsPlayerAsync(int playerId)
    {
        return await _apiClient.GetFromJsonAsync<List<PronosticDto>>(
            $"pronostic/pronostics/player/{playerId}") ?? new();
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
    
    public async Task<PlayerJokersDto> GetMyJokersAsync()
    {
        return await _apiClient.GetFromJsonAsync<PlayerJokersDto>(
            "pronostic/jokers") ?? new();
    }

    public async Task<PlayerJokersDto> PlaceJokerAsync(int pronosticId)
    {
        var response = await _apiClient.PostAsJsonAsync(
            $"pronostic/jokers/place/{pronosticId}", new { });
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlayerJokersDto>()
            ?? throw new Exception("Erreur lors du placement du joker");
    }

    public async Task<PlayerJokersDto> RemoveJokerAsync(int pronosticId)
    {
        var response = await _apiClient.DeleteAsync(
            $"pronostic/jokers/remove/{pronosticId}");
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlayerJokersDto>()
            ?? throw new Exception("Erreur lors du retrait du joker");
    }
}