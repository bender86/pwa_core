using System.Net.Http.Json;
using dto.worldbet.Models;
using dto.worldbet.Requests;
namespace PWA.Auth.Services;

public class SpecialBetService : ISpecialBetService
{
    private readonly WorldBetApiClient _apiClient;
    private readonly ILogger<SpecialBetService> _logger;

    public SpecialBetService(WorldBetApiClient apiClient, ILogger<SpecialBetService> logger)
    {
        _apiClient = apiClient;
        _logger = logger;
    }

    public async Task<SpecialBetStatusDto?> GetWinnerFinalStatusAsync()
    {
        try
        {
            return await _apiClient.GetFromJsonAsync<SpecialBetStatusDto>("specialbets/winnerfinal");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching winner final status");
            throw;
        }
    }

    public async Task<bool> UpsertWinnerFinalAsync(string chosenValue)
    {
        try
        {
            var response = await _apiClient.PostAsJsonAsync(
                "specialbets/winnerfinal",
                new UpsertSpecialBetRequest { ChosenValue = chosenValue });

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving winner final bet");
            throw;
        }
    }
}