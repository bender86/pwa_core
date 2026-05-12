
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net;
using System.Net.Http.Headers;

namespace PWA.Auth.Handlers
{
    public class CustomAuthorizationHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly NavigationManager _navigation;

        public CustomAuthorizationHandler(
            ILocalStorageService localStorage,
            NavigationManager navigation)
        {
            _localStorage = localStorage;
            _navigation   = navigation;
        }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, 
            CancellationToken cancellationToken)
        {
            // R�cup�rer le token depuis localStorage
            var token = await _localStorage.GetItemAsStringAsync("authToken", cancellationToken);

            if (!string.IsNullOrWhiteSpace(token))
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await base.SendAsync(request, cancellationToken);

            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                await _localStorage.RemoveItemAsync("authToken", cancellationToken);
                _navigation.NavigateTo("/login", forceLoad: false);
            }

            return response;
        }
    }
}