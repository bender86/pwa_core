using dto.worldbet.Models;
using dto.worldbet.Requests;

namespace PWA.Auth.Services;

public interface IPronosticService
{
    Task<PronosticDto> CreateOrUpdatePronosticAsync(CreatePronosticRequest request);
    Task<List<PronosticDto>> GetMyPronosticsAsync();
    Task<List<PronosticDto>> GetMyPronosticsPlayerAsync(int playerId);
    Task<PronosticDto?> GetMyPronosticForMatchAsync(int matchId);
    Task<MatchPronosticsDto> GetMatchPronosticsAsync(int matchId);
    Task DeletePronosticAsync(int pronosticId);
}