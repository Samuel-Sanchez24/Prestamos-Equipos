using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Prestamos.Web;
using Prestamos.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Dirección de la API: se configura en wwwroot/appsettings.json
var urlApi = builder.Configuration["ApiUrl"] ?? "http://localhost:5276";

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(urlApi) });
builder.Services.AddScoped<ApiCliente>();

await builder.Build().RunAsync();   