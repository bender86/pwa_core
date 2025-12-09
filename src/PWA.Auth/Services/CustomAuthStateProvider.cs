using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace PWA.Auth.Services;

/// <summary>
/// Custom authentication state provider for Blazor WebAssembly
/// </summary>
public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly IAuthService _authService;
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public CustomAuthStateProvider(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Get current authentication state
    /// </summary>
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        Console.WriteLine("==> GetAuthenticationStateAsync called");
        
        try
        {
            var token = await _authService.GetTokenAsync();
            Console.WriteLine($"==> Token: {(string.IsNullOrEmpty(token) ? "null" : token.Substring(0, Math.Min(10, token.Length)) + "...")}");
            
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine("==> No token, returning anonymous");
                return _anonymous;
            }

            var user = await _authService.GetCurrentUserAsync();
            Console.WriteLine($"==> User: {user?.Email ?? "null"}");
            
            if (user == null)
            {
                Console.WriteLine("==> No user, returning anonymous");
                return _anonymous;
            }

            // Create claims for the user
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Email, user.Email ?? string.Empty),
                new("token", token)
            };

            // Add admin role if applicable
            if (user.Admin == 1)
            {
                claims.Add(new Claim(ClaimTypes.Role, "Admin"));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var claimsPrincipal = new ClaimsPrincipal(identity);

            Console.WriteLine("==> Returning authenticated state");
            return new AuthenticationState(claimsPrincipal);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"==> ERROR in GetAuthenticationStateAsync: {ex.Message}");
            return _anonymous;
        }
    }

    /// <summary>
    /// Notify that authentication state has changed (after login)
    /// </summary>
    public void NotifyUserAuthentication()
    {
        var authState = GetAuthenticationStateAsync();
        NotifyAuthenticationStateChanged(authState);
    }

    /// <summary>
    /// Notify that user has logged out
    /// </summary>
    public void NotifyUserLogout()
    {
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
    }
}