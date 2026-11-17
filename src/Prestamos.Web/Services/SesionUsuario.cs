using System.Text.Json;
using Microsoft.JSInterop;
using Prestamos.Contracts.Auth;
using Prestamos.Contracts.Usuarios;

namespace Prestamos.Web.Services;

/// <summary>
/// Guarda el token y los datos del usuario autenticado.
/// Se conserva en localStorage para no perder la sesión al recargar la página.
/// </summary>
public class SesionUsuario
{
    private const string Clave = "prestamos.sesion";
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly IJSRuntime _js;

    public SesionUsuario(IJSRuntime js)
    {
        _js = js;
    }

    public string? Token { get; private set; }
    public DateTime ExpiraEn { get; private set; }
    public UsuarioResponse? Usuario { get; private set; }

    public bool EstaAutenticado => Token is not null && Usuario is not null && ExpiraEn > DateTime.UtcNow;
    public string Rol => Usuario?.Rol ?? string.Empty;
    public string Nombre => Usuario?.Nombre ?? string.Empty;
    public int UsuarioId => Usuario?.Id ?? 0;

    public bool EsRol(params string[] roles) => EstaAutenticado && roles.Contains(Rol);

    /// <summary>Se dispara al iniciar o cerrar sesión (para refrescar menú y barra superior).</summary>
    public event Action? Cambio;

    public async Task InicializarAsync()
    {
        try
        {
            var json = await _js.InvokeAsync<string?>("localStorage.getItem", Clave);
            if (string.IsNullOrWhiteSpace(json)) return;

            var sesion = JsonSerializer.Deserialize<SesionResponse>(json, Json);
            if (sesion is null || sesion.ExpiraEn <= DateTime.UtcNow)
            {
                await _js.InvokeVoidAsync("localStorage.removeItem", Clave);
                return;
            }
            Asignar(sesion);
        }
        catch
        {
            // Si el almacenamiento no está disponible, se continúa sin sesión
        }
    }

    public async Task IniciarAsync(SesionResponse sesion)
    {
        Asignar(sesion);
        try { await _js.InvokeVoidAsync("localStorage.setItem", Clave, JsonSerializer.Serialize(sesion, Json)); }
        catch { /* sin persistencia */ }
        Cambio?.Invoke();
    }

    public async Task CerrarAsync()
    {
        Token = null;
        Usuario = null;
        ExpiraEn = DateTime.MinValue;
        try { await _js.InvokeVoidAsync("localStorage.removeItem", Clave); }
        catch { /* sin persistencia */ }
        Cambio?.Invoke();
    }

    private void Asignar(SesionResponse sesion)
    {
        Token = sesion.Token;
        ExpiraEn = sesion.ExpiraEn;
        Usuario = sesion.Usuario;
    }
}