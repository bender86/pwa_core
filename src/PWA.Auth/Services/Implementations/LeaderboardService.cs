using dto.worldbet.Models;

namespace PWA.Auth.Services;

public class LeaderboardService : ILeaderboardService
{
    private readonly WorldBetApiClient _apiClient;

    public LeaderboardService(WorldBetApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<LeaderboardDto>> GetLeaderboardAsync()
    {
        return await _apiClient.GetFromJsonAsync<List<LeaderboardDto>>(
            "leaderboard") ?? new();
    }

    public async Task<LeaderboardDto?> GetPlayerRankingAsync(int playerId)
    {
        return await _apiClient.GetFromJsonAsync<LeaderboardDto>(
            $"leaderboard/player/{playerId}");
    }
}