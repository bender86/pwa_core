using dto.auth.Models;
using dto.common.Models;

namespace PWA.Auth.Services
{
    public interface IMfaService
    {
        /// <summary>
        /// Initialiser le setup MFA (génère QR code)
        /// </summary>
        Task<ApiResponse<MfaSetupResponse>> SetupMfa(int userId);

        /// <summary>
        /// Activer MFA après vérification du code
        /// </summary>
        Task<ApiResponse<object>> EnableMfa(int userId, string code);

        /// <summary>
        /// Désactiver MFA après vérification du code
        /// </summary>
        Task<ApiResponse<object>> DisableMfa(int userId, string code);
    }
}