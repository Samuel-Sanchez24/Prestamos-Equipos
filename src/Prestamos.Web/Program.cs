using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using Prestamos.Web;
using Prestamos.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// URL de la API (wwwroot/appsettings.json)
var apiUrl = builder.Configuration["ApiUrl"] ?? "https://localhost:7011/";

builder.Services.AddMudServices();
builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(apiUrl) });
builder.Services.AddSingleton<SesionUsuario>();
builder.Services.AddSingleton<RegistroErrores>();
builder.Services.AddScoped<ApiCliente>();

var host = builder.Build();

// Recupera la sesión guardada en el navegador (si el token sigue vigente)
await host.Services.GetRequiredService<SesionUsuario>().InicializarAsync();

await host.RunAsync();