using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Components;
using Microsoft.Win32;

namespace Prestamos.Web.Services;

/// <summary>
/// Único punto de comunicación con la API REST: agrega el token, serializa JSON
/// e interpreta los errores ProblemDetails (400, 401, 403, 404, 409).
/// </summary>
public class ApiCliente
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly HttpClient _http;
    private readonly SesionUsuario _sesion;
    private readonly NavigationManager _nav;
    private readonly RegistroErrores _registro;

    public ApiCliente(HttpClient http, SesionUsuario sesion, NavigationManager nav, RegistroErrores registro)
    {
        _http = http;
        _sesion = sesion;
        _nav = nav;
        _registro = registro;
    } 

    public Task<Resultado<T>> GetAsync<T>(string url) => EnviarAsync<T>(HttpMethod.Get, url, null);
    public Task<Resultado<T>> PostAsync<T>(string url, object? cuerpo = null) => EnviarAsync<T>(HttpMethod.Post, url, cuerpo);
    public Task<Resultado<T>> PutAsync<T>(string url, object cuerpo) => EnviarAsync<T>(HttpMethod.Put, url, cuerpo);
    public Task<Resultado<T>> PatchAsync<T>(string url, object cuerpo) => EnviarAsync<T>(HttpMethod.Patch, url, cuerpo);

    private async Task<Resultado<T>> EnviarAsync<T>(HttpMethod metodo, string url, object? cuerpo)
    {
        try
        {
            using var solicitud = new HttpRequestMessage(metodo, url);
            if (_sesion.Token is not null)
                solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _sesion.Token);
            if (cuerpo is not null)
                solicitud.Content = JsonContent.Create(cuerpo, cuerpo.GetType(), options: Json);

            using var respuesta = await _http.SendAsync(solicitud);
            var codigo = (int)respuesta.StatusCode;

            if (respuesta.IsSuccessStatusCode)
            {
                var texto = await respuesta.Content.ReadAsStringAsync();
                var datos = string.IsNullOrWhiteSpace(texto) ? default : JsonSerializer.Deserialize<T>(texto, Json);
                return Resultado<T>.Ok(datos, codigo);
            }

            var problema = await LeerProblemaAsync(respuesta);
            _registro.Registrar("API", codigo, problema.Titulo, problema.Mensaje, $"{metodo} {url}");

            // Error interno del servidor: se lleva a la página de error
            if (codigo >= 500)
            {
                _nav.NavigateTo($"error?codigo={codigo}");
                return Resultado<T>.Fallo(codigo, problema);
            }

            // Sesión vencida o inválida: se cierra y se envía al login
            if (respuesta.StatusCode == HttpStatusCode.Unauthorized && _sesion.Token is not null && !url.Contains("auth/login"))
            {
                await _sesion.CerrarAsync();
                _nav.NavigateTo("login");
            }

            return Resultado<T>.Fallo(codigo, problema);
        }
        catch (HttpRequestException)
        {
            var problema = new Problema
            {
                Titulo = "Sin conexión con la API",
                Detalle = "No se pudo conectar con el servidor. Verifica que la API esté en ejecución (https://localhost:7011)."
            };
            _registro.Registrar("Conexión", 0, problema.Titulo, problema.Detalle, $"{metodo} {url}");
            return Resultado<T>.Fallo(0, problema);
        }
        catch (TaskCanceledException)
        {
            var problema = new Problema { Titulo = "Tiempo de espera agotado", Detalle = "La API tardó demasiado en responder." };
            _registro.Registrar("Conexión", 0, problema.Titulo, problema.Detalle, $"{metodo} {url}");
            return Resultado<T>.Fallo(0, problema);
        }
    }

    private static async Task<Problema> LeerProblemaAsync(HttpResponseMessage respuesta)
    {
        var problema = new Problema { Titulo = TituloPorCodigo(respuesta.StatusCode) };
        try
        {
            var texto = await respuesta.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(texto)) return problema;

            var dto = JsonSerializer.Deserialize<ProblemaDto>(texto, Json);
            if (dto is null) return problema;

            if (!string.IsNullOrWhiteSpace(dto.Title)) problema.Titulo = dto.Title;
            problema.Detalle = dto.Detail;
            if (dto.Errors is not null)
                problema.Errores = dto.Errors.SelectMany(e => e.Value).Distinct().ToList();
        }
        catch (JsonException)
        {
            // Cuerpo no es ProblemDetails: se deja el título por código
        }
        return problema;
    }

    private static string TituloPorCodigo(HttpStatusCode codigo) => codigo switch
    {
        HttpStatusCode.BadRequest => "Datos inválidos",
        HttpStatusCode.Unauthorized => "Sesión no válida. Inicia sesión de nuevo.",
        HttpStatusCode.Forbidden => "No tienes permiso para realizar esta acción.",
        HttpStatusCode.NotFound => "No encontrado",
        HttpStatusCode.Conflict => "La operación no es posible",
        _ => "Ocurrió un error inesperado"
    };

    private sealed class ProblemaDto
    {
        public string? Title { get; set; }
        public string? Detail { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
    }
}