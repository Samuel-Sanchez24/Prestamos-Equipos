using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Prestamos.Web;
using Prestamos.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Configuración en wwwroot/appsettings.json
var urlApi = builder.Configuration["ApiUrl"] ?? "http://localhost:5276";
var modoDemo = string.Equals(builder.Configuration["ModoDemo"], "true", StringComparison.OrdinalIgnoreCase);

if (modoDemo)
    builder.Services.AddScoped(sp => new HttpClient(new ApiDemoHandler()) { BaseAddress = new Uri(urlApi) });
else
    builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(urlApi) });

builder.Services.AddScoped<ApiCliente>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<ProveedorAutenticacion>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => sp.GetRequiredService<ProveedorAutenticacion>());
builder.Services.AddScoped<SesionServicio>();

var host = builder.Build();

// Recupera la sesión guardada en el navegador antes de mostrar la primera pantalla
await host.Services.GetRequiredService<SesionServicio>().RestaurarAsync();

await host.RunAsync();