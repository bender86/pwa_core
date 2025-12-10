using System.Net.Http.Json;
using dto.user.Models;
using dto.common.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace PWA.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<UserService> _logger;

        public UserService(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            ILogger<UserService> logger)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _logger = logger;
        }

        public async Task<ApiResponse<UserResponse>> CreateUser(CreateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Création d'un utilisateur: {Email}", request.Email);

                var response = await _httpClient.PostAsJsonAsync("user", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de création utilisateur: {StatusCode}", response.StatusCode);
                    return ApiResponse<UserResponse>.ErrorResponse(
                        "Erreur lors de la création de l'utilisateur",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
                return result ?? ApiResponse<UserResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création d'utilisateur");
                return ApiResponse<UserResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<UserResponse>> UpdateUser(UpdateUserRequest request)
        {
            try
            {
                _logger.LogInformation("Mise à jour utilisateur: {UserId}", request.Id);

                var response = await _httpClient.PutAsJsonAsync($"user/{request.Id}", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec de mise à jour: {StatusCode}", response.StatusCode);
                    return ApiResponse<UserResponse>.ErrorResponse(
                        "Erreur lors de la mise à jour",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserResponse>>();
                return result ?? ApiResponse<UserResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour utilisateur");
                return ApiResponse<UserResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<UserDetailResponse>> GetUserById(int id)
        {
            try
            {
                _logger.LogInformation("Récupération utilisateur: {UserId}", id);

                var response = await _httpClient.GetAsync($"user/{id}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Utilisateur non trouvé: {UserId}", id);
                    return ApiResponse<UserDetailResponse>.ErrorResponse(
                        "Utilisateur non trouvé",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDetailResponse>>();
                return result ?? ApiResponse<UserDetailResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération utilisateur");
                return ApiResponse<UserDetailResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<List<UserResponse>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les utilisateurs");

                var response = await _httpClient.GetAsync("user");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec récupération utilisateurs: {StatusCode}", response.StatusCode);
                    return ApiResponse<List<UserResponse>>.ErrorResponse(
                        "Erreur lors de la récupération",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserResponse>>>();
                return result ?? ApiResponse<List<UserResponse>>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return ApiResponse<List<UserResponse>>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                _logger.LogInformation("Changement de mot de passe: {UserId}", request.UserId);

                var response = await _httpClient.PostAsJsonAsync(
                    $"user/{request.UserId}/change-password",
                    request
                );
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec changement mot de passe: {StatusCode}", response.StatusCode);
                    return ApiResponse<object>.ErrorResponse(
                        "Erreur lors du changement de mot de passe",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                return result ?? ApiResponse<object>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du changement de mot de passe");
                return ApiResponse<object>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<UserDetailResponse?> GetCurrentUser()
        {
            try
            {
                var authState = await _authStateProvider.GetAuthenticationStateAsync();
                var user = authState.User;

                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    return null;
                }

                // Récupérer l'ID de l'utilisateur depuis les claims
                var userIdClaim = user.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                {
                    _logger.LogWarning("UserId claim non trouvé ou invalide");
                    return null;
                }

                var result = await GetUserById(userId);
                return result.Success ? result.Data : null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur actuel");
                return null;
            }
        }
    }
}