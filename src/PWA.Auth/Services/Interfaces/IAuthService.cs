
using dto.auth.Models;
using dto.common.Models;

namespace PWA.Auth.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<AuthenticateResponse>> LoginAsync(string email, string password);
    Task<ApiResponse<AuthenticateResponse>> VerifyMfaAsync(string sessionId, string code);
    Task<ApiResponse<MfaSetupResponse>> EnableMfaAsync(int userId);
    Task<ApiResponse<object>> ConfirmMfaSetupAsync(int userId, string code);
    Task<ApiResponse<object>> DisableMfaAsync(int userId, string mfaCode);
    Task LogoutAsync();
    // Token management
    Task<string?> GetTokenAsync();
    Task StoreTokenAsync(string token);
    Task RemoveTokenAsync();
     // User info
    Task<AuthenticateResponse?> GetCurrentUserAsync();
    // Task<List<string>> GetUserApplicationsAsync();
    Task<List<ApplicationDetailsResponse>> GetUserApplicationDetailsAsync();
    Task<bool> IsAuthenticatedAsync();
}