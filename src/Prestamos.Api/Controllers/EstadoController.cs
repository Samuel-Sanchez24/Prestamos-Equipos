using Microsoft.AspNetCore.Mvc;
using Prestamos.Persistence.Context;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/estado")]
public class EstadoController : ControllerBase
{
    private readonly PrestamosDbContext _db;

    public EstadoController(PrestamosDbContext db)
    {
        _db = db;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var conectada = await _db.Database.CanConnectAsync();
        return Ok(new { api = "En ejecución", baseDeDatos = conectada ? "Conectada" : "Sin conexión" });
    }
}