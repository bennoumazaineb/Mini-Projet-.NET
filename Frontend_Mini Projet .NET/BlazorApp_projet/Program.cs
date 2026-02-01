using BlazorApp_projet;
using BlazorApp1.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Logging;
using System.Net.Http;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

// Ajouter le composant principal Blazor
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configurer HttpClient pour appeler le gateway ou l'API
builder.Services.AddScoped(sp =>
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:7191")
    };
    return httpClient;
});
builder.Services.AddScoped(sp =>
    new HttpClient
    {
        BaseAddress = new Uri("http://localhost:5241/")
    });

// Enregistrer les services personnalisés
builder.Services.AddScoped<IApiService, ApiService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRoleService, RoleService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IInterventionService, InterventionService>();
builder.Services.AddScoped<IReclamationService, ReclamationService>();

// Ajouter le logger
builder.Logging.SetMinimumLevel(LogLevel.Debug); // Niveau minimum des logs

await builder.Build().RunAsync();
