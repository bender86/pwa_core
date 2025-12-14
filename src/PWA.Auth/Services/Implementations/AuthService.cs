
using dto.auth.Models;
using dto.common.Models;
using System.Net.Http.Json;
using Blazored.LocalStorage;

namespace PWA.Auth.Services;

/// <summary>
/// Authentication service to communicate with MaesAuth API
/// </summary>
public class AuthService : IAuthService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalStorageService _localStorage;
    private const string TokenKey = "authToken";
    private const string UserKey = "currentUser";

    public AuthService(HttpClient httpClient, ILocalStorageService localStorage)
    {
        _httpClient = httpClient;
        _localStorage = localStorage;
    }

    /// <summary>
    /// Initial authentication (email + password)
    /// Endpoint: POST /user/authenticate
    /// </summary>
    public async Task<ApiResponse<AuthenticateResponse>> LoginAsync(string email, string password)
    {
        try
        {
            var request = new AuthenticateRequest
            {
                Email = email,
                Password = password
            };
            // ? LOG pour debug
            Console.WriteLine($"==> BaseAddress: {_httpClient.BaseAddress}");
            Console.WriteLine($"==> Full URL will be: {_httpClient.BaseAddress}user/authenticate");
            

            var response = await _httpClient.PostAsJsonAsync("user/authenticate", request);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return ApiResponse<AuthenticateResponse>.ErrorResponse(
                    $"Authentication error: {response.StatusCode}", 
                    (int)response.StatusCode
                );
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthenticateResponse>>();
            
            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                Console.WriteLine($"==> apiResponse.Data: {apiResponse.Data}");
                // If MFA is not required, store the token
                if (!apiResponse.Data.RequiresMfa && !string.IsNullOrEmpty(apiResponse.Data.Token))
                {
                    await StoreAuthDataAsync(apiResponse.Data.Token, apiResponse.Data);
                }
            }

            return apiResponse ?? ApiResponse<AuthenticateResponse>.ErrorResponse("Invalid response");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthenticateResponse>.ErrorResponse($"Error: {ex.Message}");
        }
    }
    

    /// <summary>
    /// Verify MFA code after login
    /// Endpoint: POST /user/verify-mfa
    /// </summary>
    public async Task<ApiResponse<AuthenticateResponse>> VerifyMfaAsync(string sessionId, string code)
    {
        try
        {
            var request = new MfaVerifyRequest 
            {
                SessionId = sessionId,
                Code = code
            };

            var response = await _httpClient.PostAsJsonAsync("user/verify-mfa", request);
            
            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<AuthenticateResponse>.ErrorResponse(
                    $"MFA verification error: {response.StatusCode}", 
                    (int)response.StatusCode
                );
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<AuthenticateResponse>>();
            
            if (apiResponse?.Success == true && apiResponse.Data != null)
            {
                Console.WriteLine($"==> apiResponse.Data: {apiResponse.Data}");
                // If MFA is not required, store the token
                if (!apiResponse.Data.RequiresMfa && !string.IsNullOrEmpty(apiResponse.Data.Token))
                {
                    await StoreAuthDataAsync(apiResponse.Data.Token, apiResponse.Data);
                }
            }

            return apiResponse ?? ApiResponse<AuthenticateResponse>.ErrorResponse("Invalid response");
        }
        catch (Exception ex)
        {
            return ApiResponse<AuthenticateResponse>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Enable MFA for logged-in user
    /// Endpoint: POST /user/enable-mfa
    /// </summary>
    public async Task<ApiResponse<MfaSetupResponse>> EnableMfaAsync(int userId)
    {
        try
        {
            var request = new EnableMfaRequest { UserId = userId };

            // Add JWT token to headers
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync("user/enable-mfa", request);
            
            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<MfaSetupResponse>.ErrorResponse(
                    $"MFA activation error: {response.StatusCode}", 
                    (int)response.StatusCode
                );
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<MfaSetupResponse>>();
            return apiResponse ?? ApiResponse<MfaSetupResponse>.ErrorResponse("Invalid response");
        }
        catch (Exception ex)
        {
            return ApiResponse<MfaSetupResponse>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Confirm MFA setup with first code
    /// Endpoint: POST /user/confirm-mfa-setup
    /// </summary>
    public async Task<ApiResponse<object>> ConfirmMfaSetupAsync(int userId, string code)
    {
        try
        {
            var request = new ConfirmMfaSetupRequest
            {
                UserId = userId,
                Code = code
            };

            // Add JWT token to headers
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync("user/confirm-mfa-setup", request);
            
            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<object>.ErrorResponse(
                    $"MFA confirmation error: {response.StatusCode}", 
                    (int)response.StatusCode
                );
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return apiResponse ?? ApiResponse<object>.ErrorResponse("Invalid response");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Disable MFA
    /// Endpoint: POST /user/disable-mfa
    /// </summary>
    public async Task<ApiResponse<object>> DisableMfaAsync(int userId, string mfaCode)
    {
        try
        {
            var user = await GetCurrentUserAsync();
            if (user == null)
            {
                return ApiResponse<object>.ErrorResponse("User not logged in");
            }

            var request = new DisableMfaRequest
            {
                UserId = userId,
                Code = mfaCode
            };

            // Add JWT token to headers
            var token = await GetTokenAsync();
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = 
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }

            var response = await _httpClient.PostAsJsonAsync("user/disable-mfa", request);
            
            if (!response.IsSuccessStatusCode)
            {
                return ApiResponse<object>.ErrorResponse(
                    $"MFA deactivation error: {response.StatusCode}", 
                    (int)response.StatusCode
                );
            }

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
            return apiResponse ?? ApiResponse<object>.ErrorResponse("Invalid response");
        }
        catch (Exception ex)
        {
            return ApiResponse<object>.ErrorResponse($"Error: {ex.Message}");
        }
    }

    /// <summary>
    /// Logout - Remove local data
    /// </summary>
    public async Task LogoutAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
        await _localStorage.RemoveItemAsync(UserKey);
        _httpClient.DefaultRequestHeaders.Authorization = null;
    }

    /// <summary>
    /// Get stored JWT token
    /// </summary>
    public async Task<string?> GetTokenAsync()
    {
        return await _localStorage.GetItemAsStringAsync(TokenKey);
    }
    /// <summary>
    /// Stocker le token JWT
    /// </summary>
    public async Task StoreTokenAsync(string token)
    {
        await _localStorage.SetItemAsStringAsync(TokenKey, token);
    }

    /// <summary>
    /// Supprimer le token JWT
    /// </summary>
    public async Task RemoveTokenAsync()
    {
        await _localStorage.RemoveItemAsync(TokenKey);
    }

    /// <summary>
    /// Get current logged-in user
    /// </summary>
    public async Task<AuthenticateResponse?> GetCurrentUserAsync()
    {
        return await _localStorage.GetItemAsync<AuthenticateResponse>(UserKey);
    }

    /// <summary>
    /// Check if user is authenticated
    /// </summary>
    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await GetTokenAsync();
        return !string.IsNullOrEmpty(token);
    }

    /// <summary>
    /// Get the list of roles for the current user
    /// </summary>
    public async Task<List<string>> GetUserRolesAsync()
    {
        try
        {
            var authData = await _localStorage.GetItemAsync<AuthenticateResponse>("authData");
            return authData?.Roles ?? new List<string>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user roles: {ex.Message}");
            return new List<string>();
        }
    }
    /// <summary>
    /// Get the list of applications the user has access to
    /// </summary>
    public async Task<List<ApplicationDetailsResponse>> GetUserApplicationDetailsAsync()
    {
        try
        {
            var authData = await _localStorage.GetItemAsync<AuthenticateResponse>(UserKey);
            return authData?.ApplicationsDetails ?? new List<ApplicationDetailsResponse>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error getting user applications: {ex.Message}");
            return new List<ApplicationDetailsResponse>();
        }
    }

    /// <summary>
    /// Check if the current user has a specific role
    /// </summary>
    /// <param name="roleName">Role name to check (case-insensitive)</param>
    public async Task<bool> HasRoleAsync(string roleName)
    {
        var roles = await GetUserRolesAsync();
        return roles.Any(r => r.Equals(roleName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Check if the current user has access to a specific application
    /// </summary>
    /// <param name="appName">Application name to check (case-insensitive)</param>
    public async Task<bool> HasApplicationAccessAsync(string appName)
    {
        var apps = await GetUserApplicationDetailsAsync();
        return apps.Any(a => a.Name.Equals(appName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    /// Check if the current user is an admin
    /// </summary>
    public async Task<bool> IsAdminAsync()
    {
        try
        {
            var authData = await _localStorage.GetItemAsync<AuthenticateResponse>("authData");
            return authData?.Admin == true;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Store authentication data
    /// </summary>
    private async Task StoreAuthDataAsync(string token, AuthenticateResponse user)
    {
        await _localStorage.SetItemAsStringAsync(TokenKey, token);
        await _localStorage.SetItemAsync(UserKey, user);
    }
}

