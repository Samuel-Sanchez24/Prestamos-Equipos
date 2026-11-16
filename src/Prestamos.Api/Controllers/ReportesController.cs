using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Application.Services;
using Prestamos.Contracts.Reportes;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/reportes")]
[Authorize(Roles = "Coordinacion,Tecnico")]
public class ReportesController : ControllerBase
{
    private readonly ReporteServicio _reportes;

    public ReportesController(ReporteServicio reportes)
    {
        _reportes = reportes;
    }

    [HttpGet("resumen")]
    [ProducesResponseType<ResumenResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Resumen([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta,
        [FromQuery] int? categoriaId, CancellationToken ct) =>
        Ok(await _reportes.ObtenerResumenAsync(desde, hasta, categoriaId, ct));

    [HttpGet("prestamos")]
    [ProducesResponseType<List<PrestamoReporteResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Prestamos([FromQuery] DateOnly? desde, [FromQuery] DateOnly? hasta,
        [FromQuery] int? categoriaId, [FromQuery] string? estado, CancellationToken ct) =>
        Ok(await _reportes.ListarPrestamosAsync(desde, hasta, categoriaId, estado, ct));
}