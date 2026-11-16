using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Usuarios;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class UsuarioServicio
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IHashContrasena _hash;
    private readonly IReloj _reloj;
    private readonly IValidator<CrearPersonalRequest> _validador;

    public UsuarioServicio(IUsuarioRepositorio usuarios, IUnidadDeTrabajo uow, IHashContrasena hash,
        IReloj reloj, IValidator<CrearPersonalRequest> validador)
    {
        _usuarios = usuarios;
        _uow = uow;
        _hash = hash;
        _reloj = reloj;
        _validador = validador;
    }

    public async Task<UsuarioResponse> CrearPersonalAsync(CrearPersonalRequest req, CancellationToken ct = default)
    {
        await _validador.ValidarOFallarAsync(req, ct);

        var correo = req.Correo.Trim().ToLowerInvariant();

        if (await _usuarios.ExisteCorreoAsync(correo, ct))
            throw new BusinessRuleException("Ya existe una cuenta registrada con ese correo.");
        if (await _usuarios.ExisteDocumentoAsync(req.Documento, ct))
            throw new BusinessRuleException("Ya existe una cuenta registrada con ese documento.");

        var usuario = new Usuario
        {
            Nombre = req.Nombre.Trim(),
            Documento = req.Documento,
            Correo = correo,
            PasswordHash = _hash.Generar(req.Contrasena),
            Rol = Enum.Parse<Rol>(req.Rol),
            Activo = true,
            FechaRegistro = _reloj.Ahora
        };

        _usuarios.Agregar(usuario);
        await _uow.GuardarCambiosAsync(ct);

        return usuario.ARespuesta();
    }

    public async Task<List<UsuarioResponse>> ListarAsync(string? rol, CancellationToken ct = default)
    {
        Rol? filtro = null;
        if (!string.IsNullOrWhiteSpace(rol))
        {
            if (!Enum.TryParse<Rol>(rol, true, out var r))
                throw new DatosInvalidosException("rol", "El rol indicado no existe.");
            filtro = r;
        }

        var usuarios = await _usuarios.ListarAsync(filtro, ct);
        return usuarios.Select(u => u.ARespuesta()).ToList();
    }

    public async Task<UsuarioResponse> ObtenerAsync(int id, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException($"No existe el usuario con id {id}.");
        return usuario.ARespuesta();
    }

    public async Task<UsuarioResponse> ActualizarAsync(int id, ActualizarUsuarioRequest req, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(id, ct)
            ?? throw new NoEncontradoException($"No existe el usuario con id {id}.");

        if (req.TipoSolicitante is not null)
        {
            if (usuario.Rol != Rol.Solicitante)
                throw new BusinessRuleException("Solo los solicitantes tienen tipo (Estudiante o Docente).");
            if (!Enum.TryParse<TipoSolicitante>(req.TipoSolicitante, true, out var tipo))
                throw new DatosInvalidosException("tipoSolicitante", "El tipo debe ser Estudiante o Docente.");
            usuario.TipoSolicitante = tipo;
        }

        if (req.Activo is not null)
        {
            if (usuario.Rol == Rol.Administrador && req.Activo == false)
                throw new BusinessRuleException("No se puede desactivar la cuenta del administrador.");
            usuario.Activo = req.Activo.Value;
        }

        await _uow.GuardarCambiosAsync(ct);
        return usuario.ARespuesta();
    }
}