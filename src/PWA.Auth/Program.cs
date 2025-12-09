using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using PWA.Auth;
using PWA.Auth.Services;
using Blazored.LocalStorage;           
using Microsoft.AspNetCore.Components.Authorization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration de l'API - Récupérer depuis appsettings.json
var apiBaseUrl = "http://localhost:5035/api/v1/";// builder.Configuration["ApiBaseUrl"] ?? "https://maeshome.ddns.net/api/v1/";

// HttpClient configuré avec l'URL de base de l'API
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri(apiBaseUrl) 
});

// Services d'authentification
builder.Services.AddBlazoredLocalStorage(); // Stockage local pour le token
builder.Services.AddAuthorizationCore(); // Support pour [Authorize]

// Enregistrer nos services personnalisés
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();


await builder.Build().RunAsync();
