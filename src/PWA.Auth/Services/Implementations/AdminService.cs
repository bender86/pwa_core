using dto.worldbet.Requests;

namespace PWA.Auth.Services;

public class AdminService : IAdminService
{
    private readonly WorldBetApiClient _apiClient;
    private readonly ILogger<AdminService> _logger;

    public AdminService(WorldBetApiClient apiClient, ILogger<AdminService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }
    
    public async Task<bool> UpdateMatchScore(UpdateMatchScoreRequest request)
    {
        try
        {
            var response = await _apiClient.PutAsJsonAsync($"admin/match/score", request);
            
            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "Score updated successfully for match {MatchId}", 
                    request.MatchId
                );
                return true;
            }
            
            // Log the error response
            var errorContent = await response.Content.ReadAsStringAsync();
            _logger.LogWarning(
                "Failed to update score for match {MatchId}. Status: {StatusCode}, Error: {Error}", 
                request.MatchId, 
                response.StatusCode,
                errorContent
            );
            
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Exception updating score for match {MatchId}", 
                request.MatchId
            );
            return false;
        }
    }
}