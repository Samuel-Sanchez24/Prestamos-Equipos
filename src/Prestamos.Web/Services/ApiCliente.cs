using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Prestamos.Web.Services;

// Resultado de una llamada a la API: trae los datos o trae un mensaje de error.
public class RespuestaApi<T>
{
    public bool Ok { get; set; }
    public T? Datos { get; set; }
    public int CodigoHttp { get; set; }
    public string? Error { get; set; }
    public Dictionary<string, string[]>? ErroresPorCampo { get; set; }
}

// Formato de error que devuelve la API (ProblemDetails)
class ProblemaApi
{
    public string? Title { get; set; }
    public string? Detail { get; set; }
    public Dictionary<string, string[]>? Errors { get; set; }
}

public class ApiCliente
{
    static readonly JsonSerializerOptions Opciones = new(JsonSerializerDefaults.Web);

    readonly HttpClient http;

    // Token JWT del usuario. Se asigna al iniciar sesión (Fase 1).
    public string? Token { get; set; }
    // Se dispara cuando la API responde 401 con una sesión abierta (token vencido o inválido).
    public event Action? OnNoAutorizado;

    public ApiCliente(HttpClient http)
    {
        this.http = http;
    }

    public Task<RespuestaApi<T>> GetAsync<T>(string ruta) => Enviar<T>(HttpMethod.Get, ruta, null);
    public Task<RespuestaApi<T>> PostAsync<T>(string ruta, object? cuerpo = null) => Enviar<T>(HttpMethod.Post, ruta, cuerpo);
    public Task<RespuestaApi<T>> PutAsync<T>(string ruta, object? cuerpo = null) => Enviar<T>(HttpMethod.Put, ruta, cuerpo);
    public Task<RespuestaApi<T>> PatchAsync<T>(string ruta, object? cuerpo = null) => Enviar<T>(HttpMethod.Patch, ruta, cuerpo);

    async Task<RespuestaApi<T>> Enviar<T>(HttpMethod metodo, string ruta, object? cuerpo)
    {
        try
        {
            var solicitud = new HttpRequestMessage(metodo, ruta);

            if (!string.IsNullOrEmpty(Token))
                solicitud.Headers.Authorization = new AuthenticationHeaderValue("Bearer", Token);

            if (cuerpo != null)
                solicitud.Content = new StringContent(JsonSerializer.Serialize(cuerpo, Opciones), Encoding.UTF8, "application/json");

            var respuesta = await http.SendAsync(solicitud);
            var texto = await respuesta.Content.ReadAsStringAsync();
            if (respuesta.StatusCode == HttpStatusCode.Unauthorized && !string.IsNullOrEmpty(Token))
                OnNoAutorizado?.Invoke();

            if (respuesta.IsSuccessStatusCode)
            {
                var exito = new RespuestaApi<T> { Ok = true, CodigoHttp = (int)respuesta.StatusCode };
                if (!string.IsNullOrWhiteSpace(texto))
                    exito.Datos = JsonSerializer.Deserialize<T>(texto, Opciones);
                return exito;
            }

            return ConstruirError<T>(respuesta.StatusCode, texto);
        }
        catch (HttpRequestException)
        {
            return new RespuestaApi<T> { Error = "No se pudo conectar con el servidor. Verifica que la API esté en ejecución." };
        }
        catch (TaskCanceledException)
        {
            return new RespuestaApi<T> { Error = "El servidor tardó demasiado en responder. Intenta de nuevo." };
        }
        catch (JsonException)
        {
            return new RespuestaApi<T> { Error = "La respuesta del servidor no tiene el formato esperado." };
        }
    }

    static RespuestaApi<T> ConstruirError<T>(HttpStatusCode codigo, string texto)
    {
        var resultado = new RespuestaApi<T> { Ok = false, CodigoHttp = (int)codigo };

        try
        {
            if (!string.IsNullOrWhiteSpace(texto))
            {
                var problema = JsonSerializer.Deserialize<ProblemaApi>(texto, Opciones);
                resultado.Error = problema?.Detail ?? problema?.Title;
                resultado.ErroresPorCampo = problema?.Errors;
            }
        }
        catch (JsonException)
        {
            // Si el cuerpo no es JSON, se usa el mensaje por defecto de abajo.
        }

        resultado.Error ??= codigo switch
        {
            HttpStatusCode.Unauthorized => "Tu sesión no es válida. Inicia sesión de nuevo.",
            HttpStatusCode.Forbidden => "No tienes permiso para realizar esta acción.",
            HttpStatusCode.NotFound => "No se encontró lo que buscas.",
            HttpStatusCode.Conflict => "La operación no se puede realizar en el estado actual.",
            HttpStatusCode.BadRequest => "Hay datos inválidos. Revisa el formulario.",
            _ => "Ocurrió un error en el servidor. Intenta de nuevo."
        };

        return resultado;
    }
}   