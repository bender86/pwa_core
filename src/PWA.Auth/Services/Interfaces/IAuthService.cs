
using Dto.Auth.Models;
using Dto.Common.Models;

namespace PWA.Auth.Services;

/// <summary>
/// Authentication service interface
/// </summary>
public interface IAuthService
{
    Task<ApiResponse<AuthenticateResponse>> LoginAsync(string email, string password);
    Task<ApiResponse<MfaVerifyResponse>> VerifyMfaAsync(string sessionId, string code);
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
    Task<bool> IsAuthenticatedAsync();
}