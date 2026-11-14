using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Prestamos.Web.Models;

namespace Prestamos.Web.Services;

public class SesionServicio
{
    const string ClaveToken = "prestamos_token";
    const string ClaveUsuario = "prestamos_usuario";
    static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web);

    readonly ApiCliente api;
    readonly ProveedorAutenticacion proveedor;
    readonly IJSRuntime js;
    readonly NavigationManager nav;

    public UsuarioDto? Usuario { get; private set; }
    public bool EstaAutenticado => Usuario != null;

    public SesionServicio(ApiCliente api, ProveedorAutenticacion proveedor, IJSRuntime js, NavigationManager nav)
    {
        this.api = api;
        this.proveedor = proveedor;
        this.js = js;
        this.nav = nav;

        // Si la API responde 401 con una sesión abierta, el token venció o no es válido.
        api.OnNoAutorizado += () => _ = ExpirarSesionAsync();
    }

    public bool TieneRol(params string[] roles) => Usuario != null && roles.Contains(Usuario.Rol);

    // Se llama una vez al arrancar: recupera la sesión guardada en el navegador.
    public async Task RestaurarAsync()
    {
        try
        {
            var token = await js.InvokeAsync<string?>("localStorage.getItem", ClaveToken);
            var json = await js.InvokeAsync<string?>("localStorage.getItem", ClaveUsuario);
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(json)) return;

            var usuario = JsonSerializer.Deserialize<UsuarioDto>(json, Opciones);
            if (usuario != null) Establecer(token, usuario);
        }
        catch (Exception)
        {
            // Si lo guardado está dañado, se arranca sin sesión.
            await BorrarAlmacenamientoAsync();
        }
    }

    public async Task<RespuestaApi<LoginRespuesta>> IniciarSesionAsync(string correo, string contrasena)
    {
        var cuerpo = new LoginSolicitud { Correo = correo, Contrasena = contrasena };
        var respuesta = await api.PostAsync<LoginRespuesta>("api/auth/login", cuerpo);

        if (!respuesta.Ok) return respuesta;

        var token = respuesta.Datos?.Token;
        var usuario = respuesta.Datos?.Usuario;
        if (string.IsNullOrEmpty(token) || usuario == null)
        {
            respuesta.Ok = false;
            respuesta.Error = "La respuesta del inicio de sesión está incompleta.";
            return respuesta;
        }

        Establecer(token, usuario);
        await js.InvokeVoidAsync("localStorage.setItem", ClaveToken, token);
        await js.InvokeVoidAsync("localStorage.setItem", ClaveUsuario, JsonSerializer.Serialize(usuario, Opciones));
        return respuesta;
    }

    public Task<RespuestaApi<object>> RegistrarAsync(RegistroSolicitud datos)
        => api.PostAsync<object>("api/auth/registro", datos);

    public async Task CerrarSesionAsync()
    {
        api.Token = null;
        Usuario = null;
        proveedor.EstablecerUsuario(null);
        await BorrarAlmacenamientoAsync();
    }

    async Task ExpirarSesionAsync()
    {
        await CerrarSesionAsync();
        nav.NavigateTo("login?expirada=true");
    }

    void Establecer(string token, UsuarioDto usuario)
    {
        api.Token = token;
        Usuario = usuario;
        proveedor.EstablecerUsuario(usuario);
    }

    async Task BorrarAlmacenamientoAsync()
    {
        try
        {
            await js.InvokeVoidAsync("localStorage.removeItem", ClaveToken);
            await js.InvokeVoidAsync("localStorage.removeItem", ClaveUsuario);
        }
        catch (Exception)
        {
            // Si el navegador no permite acceder al almacenamiento, no hay nada que borrar.
        }
    }
}   