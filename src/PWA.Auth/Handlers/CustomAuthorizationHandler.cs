using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace PWA.Auth.Handlers
{
    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public CustomAuthorizationHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // R�cup�rer le token depuis localStorage
            var token = await _localStorage.GetItemAsStringAsync("authToken", cancellationToken);

            // Ajouter le header Authorization si le token existe
            if (!string.IsNullOrWhiteSpace(token))
            {
                Console.WriteLine($"Adding Bearer token: {token.Substring(0, 20)}..."); // Log pour debug
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                Console.WriteLine("?? No token found in localStorage");
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}