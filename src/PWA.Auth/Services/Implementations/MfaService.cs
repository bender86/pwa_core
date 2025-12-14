using System.Net.Http.Json;
using dto.user.Models;
using dto.auth.Models;
using dto.common.Models;

namespace PWA.Auth.Services
{
    public class MfaService : IMfaService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<MfaService> _logger;

        public MfaService(HttpClient httpClient, ILogger<MfaService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        /// <summary>
        /// Initialiser le setup MFA (génère QR code et secret)
        /// Appelle POST /api/v1/user/enable-mfa
        /// </summary>
        public async Task<ApiResponse<MfaSetupResponse>> SetupMfa(int userId)
        {
            try
            {
                _logger.LogInformation("🔐 Setup MFA pour utilisateur {UserId}", userId);

                var request = new EnableMfaRequest { UserId = userId };
                var response = await _httpClient.PostAsJsonAsync("user/enable-mfa", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Échec setup MFA: {StatusCode}", response.StatusCode);
                    return ApiResponse<MfaSetupResponse>.ErrorResponse(
                        "Erreur lors de l'initialisation MFA",
                        (int)response.StatusCode
                    );
                }

                // L'API retourne un objet avec QrCodeBase64 et SecretKey
                var result = await response.Content.ReadFromJsonAsync<ApiResponse<MfaSetupResponse>>();
                
                if (result?.Success == true && result.Data != null)
                {
                    _logger.LogInformation("? Setup MFA reussi");
                    return result;
                }
                
                return ApiResponse<MfaSetupResponse>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur SetupMfa");
                return ApiResponse<MfaSetupResponse>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Activer MFA après vérification du code
        /// Appelle POST /api/v1/user/confirm-mfa-setup
        /// </summary>
        public async Task<ApiResponse<object>> EnableMfa(int userId, string code)
        {
            try
            {
                _logger.LogInformation("🔐 Enable MFA pour utilisateur {UserId}", userId);

                var request = new ConfirmMfaSetupRequest 
                { 
                    UserId = userId,
                    Code = code 
                };
                
                var response = await _httpClient.PostAsJsonAsync("user/confirm-mfa-setup", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Échec enable MFA: {StatusCode}", response.StatusCode);
                    return ApiResponse<object>.ErrorResponse(
                        "Erreur lors de l'activation MFA",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                
                if (result?.Success == true)
                {
                    _logger.LogInformation("✅ MFA activé avec succès");
                }
                
                return result ?? ApiResponse<object>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur EnableMfa");
                return ApiResponse<object>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }

        /// <summary>
        /// Désactiver MFA après vérification du code
        /// Appelle POST /api/v1/user/disable-mfa
        /// </summary>
        public async Task<ApiResponse<object>> DisableMfa(int userId, string code)
        {
            try
            {
                _logger.LogInformation("🔐 Disable MFA pour utilisateur {UserId}", userId);

                var request = new DisableMfaRequest 
                { 
                    UserId = userId,
                    Code = code 
                };
                
                var response = await _httpClient.PostAsJsonAsync("user/disable-mfa", request);
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("❌ Échec disable MFA: {StatusCode}", response.StatusCode);
                    return ApiResponse<object>.ErrorResponse(
                        "Erreur lors de la désactivation MFA",
                        (int)response.StatusCode
                    );
                }

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                
                if (result?.Success == true)
                {
                    _logger.LogInformation("✅ MFA désactivé avec succès");
                }
                
                return result ?? ApiResponse<object>.ErrorResponse("Réponse invalide", 500);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Erreur DisableMfa");
                return ApiResponse<object>.ErrorResponse($"Erreur: {ex.Message}", 500);
            }
        }
    }
}