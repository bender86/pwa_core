using System.Net.Http.Json;
using dto.user.Models;
using dto.common.Models;
using Microsoft.AspNetCore.Components.Authorization;
using dto.user.Enums;

namespace PWA.Auth.Services
{
    public class UserService : IUserService
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationStateProvider _authStateProvider;
        private readonly ILogger<UserService> _logger;
        private readonly IAuthService _authService;

        public UserService(
            HttpClient httpClient,
            AuthenticationStateProvider authStateProvider,
            ILogger<UserService> logger, IAuthService authService)
        {
            _httpClient = httpClient;
            _authStateProvider = authStateProvider;
            _logger = logger;
            _authService = authService;
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

        public async Task<ApiResponse<List<UserDetailResponse>>> GetAllUsers()
        {
            try
            {
                _logger.LogInformation("Récupération de tous les utilisateurs");

                var response = await _httpClient.GetAsync("user");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec récupération utilisateurs: {StatusCode}", response.StatusCode);
                    return ApiResponse<List<UserDetailResponse>>.ErrorResponse(
                        "Erreur lors de la récupération",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserDetailResponse>>>();
                return result ?? ApiResponse<List<UserDetailResponse>>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des utilisateurs");
                return ApiResponse<List<UserDetailResponse>>.ErrorResponse($"Erreur: {ex.Message}", 500);
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
                _logger.LogInformation("?? Début GetCurrentUser");
                
                // ? Utiliser directement la méthode existante de AuthService
                var currentUser = await _authService.GetCurrentUserAsync();
                
                if (currentUser == null)
                {
                    _logger.LogWarning("? GetCurrentUserAsync returned null");
                    return null;
                }

                _logger.LogInformation($"? User from AuthService: {currentUser.Email}, ID: {currentUser.Id}");

                // Récupérer les détails complets depuis l'API
                var result = await GetUserById(currentUser.Id);
                
                if (result.Success && result.Data != null)
                {
                    _logger.LogInformation($"? UserDetail récupéré: {result.Data.Email}");
                    return result.Data;
                }
                else
                {
                    _logger.LogWarning($"? Erreur GetUserById: {result.ErrorMessage}");
                    
                    // Fallback : créer un UserDetailResponse basique depuis currentUser
                    return new UserDetailResponse
                    {
                        Id = currentUser.Id,
                        Email = currentUser.Email,
                        // FirstName = currentUser.FirstName,
                        // LastName = currentUser.LastName,
                        IsAdmin = currentUser.Admin ?? false,
                        Active = true,
                        MfaEnabled = currentUser.MfaEnabled ?? false,
                        Status = dto.user.Enums.UserStatus.Active,
                        CreatedAt = DateTime.UtcNow
                    };
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "? Erreur lors de la récupération de l'utilisateur actuel");
                return null;
            }
        }
        public async Task<ApiResponse<UserDetailResponse>> UpdateUserStatus(int userId, UserStatus newStatus)
        {
            try
            {
                _logger.LogInformation("Mise à jour statut utilisateur {UserId} vers {Status}", userId, newStatus);

                var response = await _httpClient.PatchAsJsonAsync(
                    $"user/{userId}/status",
                    new { Status = newStatus }
                );
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec mise à jour statut: {StatusCode}", response.StatusCode);
                    return ApiResponse<UserDetailResponse>.ErrorResponse(
                        "Erreur lors de la mise à jour du statut",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDetailResponse>>();
                return result ?? ApiResponse<UserDetailResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la mise à jour du statut");
                return ApiResponse<UserDetailResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> DeleteUser(int userId)
        {
            try
            {
                _logger.LogInformation("Suppression utilisateur: {UserId}", userId);

                var response = await _httpClient.DeleteAsync($"user/{userId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec suppression: {StatusCode}", response.StatusCode);
                    return ApiResponse<bool>.ErrorResponse(
                        "Erreur lors de la suppression",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result ?? ApiResponse<bool>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression");
                return ApiResponse<bool>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<UserInvitationResponse>> CreateUserInvitation(CreateUserInvitationRequest request)
        {
            try
            {
                _logger.LogInformation("Création d'invitation utilisateur: {Email}", request.Email);

                var response = await _httpClient.PostAsJsonAsync("user/invitation", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec création invitation: {StatusCode}", response.StatusCode);
                    return ApiResponse<UserInvitationResponse>.ErrorResponse(
                        "Erreur lors de la création de l'invitation",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserInvitationResponse>>();
                return result ?? ApiResponse<UserInvitationResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la création d'invitation");
                return ApiResponse<UserInvitationResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<UserDetailResponse>> CompleteUserRegistration(CompleteUserRegistrationRequest request)
        {
            try
            {
                _logger.LogInformation("Complétion d'enregistrement utilisateur avec token");

                var response = await _httpClient.PostAsJsonAsync("user/complete-registration", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Échec complétion: {StatusCode}", response.StatusCode);
                    return ApiResponse<UserDetailResponse>.ErrorResponse(
                        "Erreur lors de la complétion de l'enregistrement",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserDetailResponse>>();
                return result ?? ApiResponse<UserDetailResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la complétion");
                return ApiResponse<UserDetailResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> RequestPasswordReset(string email)
        {
            try
            {
                var request = new RequestPasswordResetRequest { Email = email };
                
                var response = await _httpClient.PostAsJsonAsync(
                    "user/request-password-reset", 
                    request
                );

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    return result ?? ApiResponse<object>.ErrorResponse("Invalid response", 500);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to request password reset: {Error}", errorContent);
                
                return ApiResponse<object>.ErrorResponse(
                    "Failed to send reset link", 
                    (int)response.StatusCode
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error requesting password reset");
                return ApiResponse<object>.ErrorResponse($"Error: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<object>> ResetPassword(ResetPasswordRequest request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync(
                    "user/reset-password", 
                    request
                );

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                    return result ?? ApiResponse<object>.ErrorResponse("Invalid response", 500);
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to reset password: {Error}", errorContent);
                
                return ApiResponse<object>.ErrorResponse(
                    "Failed to reset password", 
                    (int)response.StatusCode
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resetting password");
                return ApiResponse<object>.ErrorResponse($"Error: {ex.Message}", 500);
            }
        }

        // Services/UserService.cs - Ajouter ces méthodes é la classe existante

        public async Task<ApiResponse<List<UserApplicationDto>>> GetUserApplications(int userId)
        {
            try
            {
                _logger.LogInformation("Récupération des applications pour l'utilisateur: {UserId}", userId);

                var response = await _httpClient.GetAsync($"user/{userId}/applications");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("échec récupération applications: {StatusCode}", response.StatusCode);
                    return ApiResponse<List<UserApplicationDto>>.ErrorResponse(
                        "Erreur lors de la récupération des applications",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<List<UserApplicationDto>>>();
                return result ?? ApiResponse<List<UserApplicationDto>>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des applications");
                return ApiResponse<List<UserApplicationDto>>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<UserApplicationDto>> AssignApplication(AssignApplicationRequest request)
        {
            try
            {
                _logger.LogInformation("Attribution application {AppId} é l'utilisateur {UserId}", 
                    request.ApplicationId, request.UserId);

                var response = await _httpClient.PostAsJsonAsync("user/assign-application", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("échec attribution application: {StatusCode} - {Error}", 
                        response.StatusCode, errorContent);
                    return ApiResponse<UserApplicationDto>.ErrorResponse(
                        errorContent,
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<UserApplicationDto>>();
                return result ?? ApiResponse<UserApplicationDto>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'attribution d'application");
                return ApiResponse<UserApplicationDto>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        public async Task<ApiResponse<bool>> RemoveApplication(RemoveApplicationRequest request)
        {
            try
            {
                _logger.LogInformation("Retrait application {AppId} de l'utilisateur {UserId}", 
                    request.ApplicationId, request.UserId);

                var response = await _httpClient.PostAsJsonAsync("user/remove-application", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning("échec retrait application: {StatusCode} - {Error}", 
                        response.StatusCode, errorContent);
                    return ApiResponse<bool>.ErrorResponse(
                        errorContent,
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<bool>>();
                return result ?? ApiResponse<bool>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors du retrait d'application");
                return ApiResponse<bool>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }
    }
}