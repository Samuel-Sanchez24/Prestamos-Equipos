using Prestamos.Api.Extensions;
using Prestamos.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiBase(builder.Configuration);
builder.Services.AddPersistence(builder.Configuration.GetConnectionString("Default")!);

var app = builder.Build();

app.UseApiBase();

app.Run();