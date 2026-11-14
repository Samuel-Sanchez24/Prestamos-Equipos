using System.Net;
using System.Text;
using System.Text.Json;
using Prestamos.Web.Models;

namespace Prestamos.Web.Services;

// TEMPORAL: responde en lugar de la API mientras no exista.
// Se activa con "ModoDemo": true en wwwroot/appsettings.json.
// ELIMINAR ESTE ARCHIVO AL INTEGRAR CON LA API REAL. No forma parte de la entrega.
public class ApiDemoHandler : HttpMessageHandler
{
    record CuentaDemo(UsuarioDto Usuario, string Documento, string Contrasena);

    static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web);

    static List<CuentaDemo> cuentas = new()
    {
        new(new UsuarioDto { Id = 1, Nombre = "Ana Pérez", Correo = "ana.perez@correo.itm.edu.co", Rol = RolUsuario.Solicitante, TipoSolicitante = "Estudiante" }, "1000000001", "Clave#2026"),
        new(new UsuarioDto { Id = 2, Nombre = "Juan Gómez", Correo = "juan.gomez@correo.itm.edu.co", Rol = RolUsuario.Solicitante, TipoSolicitante = "Docente" }, "1000000002", "Clave#2026"),
        new(new UsuarioDto { Id = 3, Nombre = "Carlos Ruiz", Correo = "carlos.ruiz@correo.itm.edu.co", Rol = RolUsuario.Tecnico }, "1000000003", "Clave#2026"),
        new(new UsuarioDto { Id = 4, Nombre = "Laura Mejía", Correo = "laura.mejia@correo.itm.edu.co", Rol = RolUsuario.Coordinacion }, "1000000004", "Clave#2026"),
        new(new UsuarioDto { Id = 5, Nombre = "Administrador", Correo = "admin@correo.itm.edu.co", Rol = RolUsuario.Administrador }, "1000000005", "Clave#2026"),
    };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await Task.Delay(300, cancellationToken); // simula la espera de la red

        var ruta = request.RequestUri!.AbsolutePath.ToLower();
        var cuerpo = request.Content == null ? "" : await request.Content.ReadAsStringAsync(cancellationToken);

        if (request.Method == HttpMethod.Post && ruta == "/api/auth/login") return Login(cuerpo);
        if (request.Method == HttpMethod.Post && ruta == "/api/auth/registro") return Registro(cuerpo);

        return Responder(HttpStatusCode.NotFound, Problema(404, "No disponible", "Esta ruta no existe en el modo demostración."));
    }

    static HttpResponseMessage Login(string cuerpo)
    {
        var datos = JsonSerializer.Deserialize<LoginSolicitud>(cuerpo, Opciones);
        var cuenta = cuentas.FirstOrDefault(c =>
            c.Usuario.Correo.Equals(datos?.Correo, StringComparison.OrdinalIgnoreCase) && c.Contrasena == datos?.Contrasena);

        if (cuenta == null)
            return Responder(HttpStatusCode.Unauthorized, Problema(401, "Credenciales inválidas", "Correo o contraseña incorrectos."));

        return Responder(HttpStatusCode.OK, new LoginRespuesta { Token = "token-demo-" + cuenta.Usuario.Id, Usuario = cuenta.Usuario });
    }

    static HttpResponseMessage Registro(string cuerpo)
    {
        var datos = JsonSerializer.Deserialize<RegistroSolicitud>(cuerpo, Opciones);
        if (datos == null)
            return Responder(HttpStatusCode.BadRequest, Problema(400, "Datos inválidos", "No se recibieron datos."));

        if (datos.Contrasena.Length < 6)
            return Responder(HttpStatusCode.BadRequest, new
            {
                title = "Datos inválidos",
                status = 400,
                errors = new Dictionary<string, string[]> { ["Contrasena"] = new[] { "La contraseña debe tener al menos 6 caracteres." } }
            });

        if (cuentas.Any(c => c.Usuario.Correo.Equals(datos.Correo, StringComparison.OrdinalIgnoreCase)))
            return Responder(HttpStatusCode.Conflict, Problema(409, "Correo duplicado", "Ya existe una cuenta con ese correo."));

        if (cuentas.Any(c => c.Documento == datos.Documento))
            return Responder(HttpStatusCode.Conflict, Problema(409, "Documento duplicado", "Ya existe una cuenta con ese documento."));

        var usuario = new UsuarioDto
        {
            Id = cuentas.Count + 1,
            Nombre = datos.Nombre,
            Correo = datos.Correo,
            Rol = RolUsuario.Solicitante,
            TipoSolicitante = datos.TipoSolicitante
        };
        cuentas.Add(new CuentaDemo(usuario, datos.Documento, datos.Contrasena));
        return Responder(HttpStatusCode.Created, usuario);
    }

    static object Problema(int estado, string titulo, string detalle) => new { title = titulo, status = estado, detail = detalle };

    static HttpResponseMessage Responder(HttpStatusCode codigo, object cuerpo)
    {
        return new HttpResponseMessage(codigo)
        {
            Content = new StringContent(JsonSerializer.Serialize(cuerpo, Opciones), Encoding.UTF8, "application/json")
        };
    }
}   