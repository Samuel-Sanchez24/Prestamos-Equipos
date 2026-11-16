using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Api.Extensions;
using Prestamos.Application.Services;
using Prestamos.Contracts.Recursos;
using Prestamos.Contracts.Reportes;

namespace Prestamos.Api.Controllers;

[ApiController]
[Route("api/recursos")]
[Authorize]
public class RecursosController : ControllerBase
{
    private readonly RecursoServicio _recursos;

    public RecursosController(RecursoServicio recursos)
    {
        _recursos = recursos;
    }

    [HttpGet]
    [ProducesResponseType<List<RecursoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Listar([FromQuery] int? categoriaId, [FromQuery] DateOnly? desde,
        [FromQuery] DateOnly? hasta, [FromQuery] bool soloDisponibles = false,
        [FromQuery] bool incluirDadosDeBaja = false, CancellationToken ct = default)
    {
        var verBajas = incluirDadosDeBaja && User.IsInRole("Administrador");
        return Ok(await _recursos.ListarAsync(categoriaId, desde, hasta, soloDisponibles, verBajas, ct));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<RecursoDetalleResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Obtener(int id, CancellationToken ct) => Ok(await _recursos.ObtenerAsync(id, ct));

    [HttpPost]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType<RecursoResponse>(StatusCodes.Status201Created)]
    public async Task<IActionResult> Crear(CrearRecursoRequest req, CancellationToken ct)
    {
        var recurso = await _recursos.CrearAsync(req, User.ObtenerUsuarioId(), ct);
        return CreatedAtAction(nameof(Obtener), new { id = recurso.Id }, recurso);
    }

    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrador")]
    [ProducesResponseType<RecursoResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Actualizar(int id, ActualizarRecursoRequest req, CancellationToken ct) =>
        Ok(await _recursos.ActualizarAsync(id, req, ct));

    [HttpPatch("{id:int}/estado")]
    [Authorize(Roles = "Tecnico,Administrador")]
    [ProducesResponseType<CambioEstadoRecursoResponse>(StatusCodes.Status200OK)]
    public async Task<IActionResult> CambiarEstado(int id, CambiarEstadoRecursoRequest req, CancellationToken ct) =>
        Ok(await _recursos.CambiarEstadoAsync(id, req, User.ObtenerUsuarioId(), User.ObtenerRol(), ct));

    [HttpGet("{id:int}/movimientos")]
    [Authorize(Roles = "Tecnico,Coordinacion,Administrador")]
    [ProducesResponseType<List<MovimientoResponse>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> Movimientos(int id, CancellationToken ct) =>
        Ok(await _recursos.ListarMovimientosAsync(id, ct));
}