using dto.worldbet.Models;
using dto.worldbet.Requests;

namespace PWA.Auth.Services;

public interface ITournamentService
{
    Task<List<GroupStandingDto>> GetGroupStandingsAsync(string groupLetter);
    Task<List<MatchDto>> GetGroupMatchesAsync(string groupLetter);
    Task<List<MatchDto>> GetRecentMatchesAsync(int count = 10);
    Task<List<MatchDto>> GetUpcomingMatchesAsync(int count = 10);
    Task<MatchDto?> GetMatchAsync(int matchId);
    Task<List<MatchDto>> GetAllMatchesAsync();
    //  m�thodes pour l'admin
    Task<MatchDto> CreateMatchAsync(CreateMatchRequest request);
    Task<MatchDto> UpdateMatchAsync(int matchId, UpdateMatchRequest request);
    Task<bool> DeleteMatchAsync(int matchId);
    Task<List<TeamDto>> GetAllTeamsAsync();
    Task<List<PhaseDto>> GetAllPhasesAsync();
}