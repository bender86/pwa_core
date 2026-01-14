using System.Net.Http.Json;
using dto.worldbet.Models;
using dto.worldbet.Requests;

namespace PWA.Auth.Services;

public class TournamentService : ITournamentService
{
    private readonly WorldBetApiClient _apiClient;
    private readonly ILogger<TournamentService> _logger;
    public TournamentService(WorldBetApiClient apiClient, ILogger<TournamentService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
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

    public async Task<MatchDto> CreateMatchAsync(CreateMatchRequest request)
        {
            var response = await _apiClient.PostAsJsonAsync("tournament/matches", request);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<MatchDto>() 
                ?? throw new InvalidOperationException("Failed to create match");
        }

        public async Task<MatchDto> UpdateMatchAsync(int matchId, UpdateMatchRequest request)
        {
            var response = await _apiClient.PutAsJsonAsync($"tournament/matches/{matchId}", request);
            response.EnsureSuccessStatusCode();
            
            return await response.Content.ReadFromJsonAsync<MatchDto>() 
                ?? throw new InvalidOperationException("Failed to update match");
        }

        public async Task<bool> DeleteMatchAsync(int matchId)
        {
            try
            {
                var response = await _apiClient.DeleteAsync($"tournament/matches/{matchId}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    return false;
                
                response.EnsureSuccessStatusCode();
                return true;
            }
            catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                throw new InvalidOperationException("Cannot delete match with existing pronostics");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting match {MatchId}", matchId);
                throw;
            }
        }

        public async Task<List<TeamDto>> GetAllTeamsAsync()
        {
            try
            {
                var teams = await _apiClient.GetFromJsonAsync<List<TeamDto>>("tournament/teams");
                return teams ?? new List<TeamDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching teams");
                throw;
            }
        }

        public async Task<List<PhaseDto>> GetAllPhasesAsync()
        {
            try
            {
                var phases = await _apiClient.GetFromJsonAsync<List<PhaseDto>>("tournament/phases");
                return phases ?? new List<PhaseDto>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching phases");
                throw;
            }
        }
}