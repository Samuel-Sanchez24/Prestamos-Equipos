using Prestamos.Api.Extensions;
using Prestamos.Application;
using Prestamos.Application.Options;
using Prestamos.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiBase(builder.Configuration);
builder.Services.AddSeguridad(builder.Configuration);
builder.Services.AddApplication();
builder.Services.AddSingleton(builder.Configuration.GetSection("Prestamos").Get<OpcionesPrestamo>() ?? new OpcionesPrestamo());
builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Default")!);

var app = builder.Build();

app.UseApiBase();

app.Run();