using FluentValidation;
using Prestamos.Application.Abstractions;
using Prestamos.Application.Exceptions;
using Prestamos.Application.Validators;
using Prestamos.Contracts.Auth;
using Prestamos.Contracts.Usuarios;
using Prestamos.Domain.Entities;
using Prestamos.Domain.Enums;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Application.Services;

public class AuthServicio
{
    private readonly IUsuarioRepositorio _usuarios;
    private readonly IUnidadDeTrabajo _uow;
    private readonly IHashContrasena _hash;
    private readonly IGeneradorToken _token;
    private readonly IReloj _reloj;
    private readonly IValidator<RegistroRequest> _validadorRegistro;
    private readonly IValidator<LoginRequest> _validadorLogin;

    public AuthServicio(IUsuarioRepositorio usuarios, IUnidadDeTrabajo uow, IHashContrasena hash,
        IGeneradorToken token, IReloj reloj,
        IValidator<RegistroRequest> validadorRegistro, IValidator<LoginRequest> validadorLogin)
    {
        _usuarios = usuarios;
        _uow = uow;
        _hash = hash;
        _token = token;
        _reloj = reloj;
        _validadorRegistro = validadorRegistro;
        _validadorLogin = validadorLogin;
    }

    public async Task<UsuarioResponse> RegistrarAsync(RegistroRequest req, CancellationToken ct = default)
    {
        await _validadorRegistro.ValidarOFallarAsync(req, ct);

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
            Rol = Rol.Solicitante,
            TipoSolicitante = Enum.Parse<TipoSolicitante>(req.TipoSolicitante),
            Activo = true,
            FechaRegistro = _reloj.Ahora
        };

        _usuarios.Agregar(usuario);
        await _uow.GuardarCambiosAsync(ct);

        return usuario.ARespuesta();
    }

    public async Task<SesionResponse> IniciarSesionAsync(LoginRequest req, CancellationToken ct = default)
    {
        await _validadorLogin.ValidarOFallarAsync(req, ct);

        var usuario = await _usuarios.ObtenerPorCorreoAsync(req.Correo.Trim().ToLowerInvariant(), ct);

        if (usuario is null || !_hash.Verificar(usuario.PasswordHash, req.Contrasena))
            throw new NoAutenticadoException();

        if (!usuario.Activo)
            throw new NoAutenticadoException("La cuenta está desactivada. Comunícate con el administrador.");

        var (token, expira) = _token.Generar(usuario);

        return new SesionResponse { Token = token, ExpiraEn = expira, Usuario = usuario.ARespuesta() };
    }

    public async Task<UsuarioResponse> ObtenerActualAsync(int usuarioId, CancellationToken ct = default)
    {
        var usuario = await _usuarios.ObtenerPorIdAsync(usuarioId, ct)
            ?? throw new NoAutenticadoException("La sesión no es válida.");
        return usuario.ARespuesta();
    }
}