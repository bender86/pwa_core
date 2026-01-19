using dto.auth.Models;
using dto.common.Models;
using dto.user.Models;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace PWA.Auth.Services;

public class ApplicationService : IApplicationService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<ApplicationService> _logger;

        public ApplicationService(
            HttpClient httpClient,
            ILogger<ApplicationService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<ApiResponse<List<ApplicationDto>>> GetAllApplications()
        {
            try
            {
                _logger.LogInformation("Récupération de toutes les applications");

                var response = await _httpClient.GetAsync("application");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("échec récupération applications: {StatusCode}", response.StatusCode);
                    return ApiResponse<List<ApplicationDto>>.ErrorResponse(
                        "Erreur lors de la récupération des applications",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<ApplicationDto>>>();
                return result ?? ApiResponse<List<ApplicationDto>>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la r�cup�ration des applications");
                return ApiResponse<List<ApplicationDto>>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }
    }