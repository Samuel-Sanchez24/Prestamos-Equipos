using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Recursos;
using Prestamos.Contracts.Reportes;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class RecursoServicio
{
    private const string Entidad = "Recurso";

    private readonly IRecursoRepositorio _recursos;
    private readonly ICategoriaRepositorio _categorias;
    private readonly IMovimientoRepositorio _movimientos;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IReloj _reloj;
    private readonly IValidator<CrearRecursoRequest> _validadorCrear;
    private readonly IValidator<ActualizarRecursoRequest> _validadorActualizar;
    private readonly IValidator<CambiarEstadoRecursoRequest> _validadorEstado;

    public RecursoServicio(IRecursoRepositorio recursos, ICategoriaRepositorio categorias,
        IMovimientoRepositorio movimientos, IUnidadDeTrabajo uow, IReloj reloj,
        IValidator<CrearRecursoRequest> validadorCrear,
        IValidator<ActualizarRecursoRequest> validadorActualizar,
        IValidator<CambiarEstadoRecursoRequest> validadorEstado)
    {
        _recursos = recursos;
        _categorias = categorias;
        _movimientos = movimientos;
        _uow = uow;
        _reloj = reloj;
        _validadorCrear = validadorCrear;
        _validadorActualizar = validadorActualizar;
        _validadorEstado = validadorEstado;
    }

    public async Task<List<RecursoResponse>> ListarAsync(int? categoriaId, DateOnly? desde, DateOnly? hasta,
        bool soloDisponibles, bool incluirDadosDeBaja, CancellationToken ct = default)
    {
        if (desde.HasValue != hasta.HasValue)
            throw new DatosInvalidosException("desde", "Indica 'desde' y 'hasta' juntos, o ninguno.");

        var inicio = desde ?? _reloj.Hoy;
        var fin = hasta ?? _reloj.Hoy;
        if (fin < inicio)
            throw new DatosInvalidosException("hasta", "La fecha 'hasta' no puede ser anterior a 'desde'.");

        var recursos = await _recursos.ListarAsync(categoriaId, incluirDadosDeBaja, ct);
        var ocupados = await _recursos.ObtenerOcupadosAsync(inicio, fin, ct: ct);

        var lista = recursos
            .Select(r => r.ARespuesta(r.EstadoOperativo == EstadoRecurso.Disponible && !ocupados.Contains(r.Id)));

        if (soloDisponibles)
            lista = lista.Where(r => r.DisponibleEnRango);

        return lista.ToList();
    }

    public async Task<RecursoDetalleResponse> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var recurso = await ObtenerOFallarAsync(id, ct);
        var hoy = _reloj.Hoy;

        var ocupado = (await _recursos.ObtenerOcupadosAsync(hoy, hoy, [id], ct: ct)).Contains(id);
        var reservas = await _recursos.ObtenerReservasVigentesAsync(id, hoy, ct);

        var basico = recurso.ARespuesta(recurso.EstadoOperativo == EstadoRecurso.Disponible && !ocupado);
        return new RecursoDetalleResponse
        {
            Id = basico.Id,
            Codigo = basico.Codigo,
            Nombre = basico.Nombre,
            CategoriaId = basico.CategoriaId,
            Categoria = basico.Categoria,
            Descripcion = basico.Descripcion,
            Ubicacion = basico.Ubicacion,
            EstadoOperativo = basico.EstadoOperativo,
            DisponibleEnRango = basico.DisponibleEnRango,
            ProximasReservas = reservas.Select(s => s.AReserva()).ToList()
        };
    }

    public async Task<RecursoResponse> CrearAsync(CrearRecursoRequest req, int usuarioId, CancellationToken ct = default)
    {
        await _validadorCrear.ValidarOFallarAsync(req, ct);

        var codigo = req.Codigo.Trim().ToUpperInvariant();
        if (await _recursos.ExisteCodigoAsync(codigo, ct))
            throw new BusinessRuleException($"Ya existe un recurso con el código {codigo}.");
        if (!await _categorias.ExisteAsync(req.CategoriaId, ct))
            throw new NoEncontradoException($"No existe la categoría con id {req.CategoriaId}.");

        var recurso = new Recurso
        {
            Codigo = codigo,
            Nombre = req.Nombre.Trim(),
            CategoriaId = req.CategoriaId,
            Descripcion = req.Descripcion?.Trim(),
            Ubicacion = req.Ubicacion?.Trim(),
            EstadoOperativo = EstadoRecurso.Disponible,
            FechaRegistro = _reloj.Ahora
        };

        _recursos.Agregar(recurso);
        await _uow.GuardarCambiosAsync(ct); // Genera el Id

        _movimientos.Agregar(Movimiento.Registrar(Entidad, recurso.Id, null, recurso.EstadoOperativo.ToString(),
            usuarioId, _reloj.Ahora, "Recurso registrado"));
        await _uow.GuardarCambiosAsync(ct);

        var creado = await ObtenerOFallarAsync(recurso.Id, ct); // Con su categoría cargada
        return creado.ARespuesta(true);
    }

    public async Task<RecursoResponse> ActualizarAsync(int id, ActualizarRecursoRequest req, CancellationToken ct = default)
    {
        await _validadorActualizar.ValidarOFallarAsync(req, ct);

        var recurso = await ObtenerOFallarAsync(id, ct);
        if (!await _categorias.ExisteAsync(req.CategoriaId, ct))
            throw new NoEncontradoException($"No existe la categoría con id {req.CategoriaId}.");

        recurso.Nombre = req.Nombre.Trim();
        recurso.CategoriaId = req.CategoriaId;
        recurso.Descripcion = req.Descripcion?.Trim();
        recurso.Ubicacion = req.Ubicacion?.Trim();
        await _uow.GuardarCambiosAsync(ct);

        var actualizado = await ObtenerOFallarAsync(id, ct);
        return actualizado.ARespuesta(actualizado.EstadoOperativo == EstadoRecurso.Disponible);
    }

    public async Task<CambioEstadoRecursoResponse> CambiarEstadoAsync(int id, CambiarEstadoRecursoRequest req,
        int usuarioId, Rol rol, CancellationToken ct = default)
    {
        await _validadorEstado.ValidarOFallarAsync(req, ct);

        var recurso = await ObtenerOFallarAsync(id, ct);
        var anterior = recurso.EstadoOperativo;
        var nuevo = Enum.Parse<EstadoRecurso>(req.Estado, ignoreCase: true);

        switch (nuevo)
        {
            case EstadoRecurso.Danado:
                if (rol != Rol.Tecnico) throw new AccesoDenegadoException("Solo el técnico puede reportar un recurso como dañado.");
                recurso.MarcarDanado();
                break;
            case EstadoRecurso.Disponible:
                if (rol != Rol.Tecnico) throw new AccesoDenegadoException("Solo el técnico puede marcar un recurso como reparado.");
                recurso.MarcarReparado();
                break;
            case EstadoRecurso.DadoDeBaja:
                if (rol != Rol.Administrador) throw new AccesoDenegadoException("Solo el administrador puede dar de baja un recurso.");
                recurso.DarDeBaja();
                break;
        }

        _movimientos.Agregar(Movimiento.Registrar(Entidad, recurso.Id, anterior.ToString(), nuevo.ToString(),
            usuarioId, _reloj.Ahora, req.Observacion?.Trim()));
        await _uow.GuardarCambiosAsync(ct);

        var afectadas = new List<ReservaResumen>();
        if (nuevo != EstadoRecurso.Disponible)
        {
            afectadas = (await _recursos.ObtenerReservasVigentesAsync(id, _reloj.Hoy, ct))
                .Where(s => s.Estado is EstadoSolicitud.Pendiente or EstadoSolicitud.Aprobada)
                .Select(s => s.AReserva())
                .ToList();
        }

        return new CambioEstadoRecursoResponse
        {
            Recurso = recurso.ARespuesta(recurso.EstadoOperativo == EstadoRecurso.Disponible),
            ReservasAfectadas = afectadas
        };
    }
    public async Task<List<MovimientoResponse>> ListarMovimientosAsync(int id, CancellationToken ct = default)
    {
        await ObtenerOFallarAsync(id, ct);
        return (await _movimientos.ListarAsync(Entidad, id, ct)).Select(m => m.ARespuesta()).ToList();
    }

    private async Task<Recurso> ObtenerOFallarAsync(int id, CancellationToken ct) =>
        await _recursos.ObtenerPorIdAsync(id, ct)
        ?? throw new NoEncontradoException($"No existe el recurso con id {id}.");
}