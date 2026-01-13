using dto.worldbet.Models;

namespace PWA.Auth.Services;

public interface ILeaderboardService
{
    Task<List<LeaderboardDto>> GetLeaderboardAsync();
    Task<LeaderboardDto?> GetPlayerRankingAsync(int playerId);
}