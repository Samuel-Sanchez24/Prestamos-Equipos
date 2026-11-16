using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Prestamos.Application.Exceptions;
using Prestamos.Domain.Exceptions;

namespace Prestamos.Api.Middleware;

public class ManejadorExcepciones : IExceptionHandler
{
    private readonly ILogger<ManejadorExcepciones> _logger;

    public ManejadorExcepciones(ILogger<ManejadorExcepciones> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
    {
        var (status, titulo) = exception switch
        {
            DatosInvalidosException => (StatusCodes.Status400BadRequest, "Datos inválidos"),
            AccesoDenegadoException => (StatusCodes.Status403Forbidden, "Acceso denegado"),
            NoEncontradoException => (StatusCodes.Status404NotFound, "No encontrado"),
            BusinessRuleException => (StatusCodes.Status409Conflict, "Regla de negocio incumplida"),
            _ => (StatusCodes.Status500InternalServerError, "Error interno")
        };

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = status == 500 ? "Ocurrió un error inesperado. Intenta de nuevo." : exception.Message,
            Instance = context.Request.Path
        };

        if (exception is DatosInvalidosException invalidos)
            problema.Extensions["errors"] = invalidos.Errores;

        if (status == 500)
            _logger.LogError(exception, "Error no controlado en {Ruta}", context.Request.Path);

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problema, ct);
        return true;
    }
}