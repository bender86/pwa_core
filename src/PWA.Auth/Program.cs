using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;  // ? Important
using Blazored.LocalStorage;
using PWA.Auth;
using PWA.Auth.Services;
using PWA.Auth.Handlers;
using System.Text.Json;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app"); 
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuration de l'API
var apiBaseUrl =  builder.Configuration["ApiBaseUrl"]??"https://maeshome.ddns.net/api/v1/" ;// "http://localhost:5035/api/v1/";

// ? Configurer les options JSON globalement pour Blazor
builder.Services.Configure<JsonSerializerOptions>(options =>
{
    options.PropertyNameCaseInsensitive = true;
});
// Services d'authentification
builder.Services.AddBlazoredLocalStorage(config =>
{
    // ? Configurer Blazored.LocalStorage pour ignorer la casse aussi
    config.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
});

// ? Handler enregistr� comme Scoped (important pour ILocalStorageService)
builder.Services.AddScoped<CustomAuthorizationHandler>();

// ? HttpClient nomm� "API" avec le handler
builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<CustomAuthorizationHandler>();

// ? HttpClient par d�faut qui utilise le client "API" configur�
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

builder.Services.AddAuthorizationCore();

// Services personnalis�s
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IMfaService, MfaService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IApplicationConfigService, ApplicationConfigService>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();

await builder.Build().RunAsync();