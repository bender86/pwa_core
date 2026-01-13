using dto.worldbet.Models;

namespace PWA.Auth.Services;

public class TournamentService : ITournamentService
{
    private readonly WorldBetApiClient _apiClient;

    public TournamentService(WorldBetApiClient apiClient)
    {
        _apiClient = apiClient;
    }

    public async Task<List<GroupStandingDto>> GetGroupStandingsAsync(string groupLetter)
    {
        return await _apiClient.GetFromJsonAsync<List<GroupStandingDto>>(
            $"tournament/group/{groupLetter}/standings") ?? new();
    }

    public async Task<List<MatchDto>> GetGroupMatchesAsync(string groupLetter)
    {
        return await _apiClient.GetFromJsonAsync<List<MatchDto>>(
            $"tournament/group/{groupLetter}/matches") ?? new();
    }

    public async Task<List<MatchDto>> GetRecentMatchesAsync(int count = 10)
    {
        return await _apiClient.GetFromJsonAsync<List<MatchDto>>(
            $"tournament/matches/recent?count={count}") ?? new();
    }

    public async Task<List<MatchDto>> GetUpcomingMatchesAsync(int count = 10)
    {
        return await _apiClient.GetFromJsonAsync<List<MatchDto>>(
            $"tournament/matches/upcoming?count={count}") ?? new();
    }

    public async Task<MatchDto?> GetMatchAsync(int matchId)
    {
        return await _apiClient.GetFromJsonAsync<MatchDto>(
            $"tournament/matches/{matchId}");
    }

    public async Task<List<MatchDto>> GetAllMatchesAsync()
    {
        return await _apiClient.GetFromJsonAsync<List<MatchDto>>(
            "tournament/matches") ?? new();
    }
}