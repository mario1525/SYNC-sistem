using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;

namespace Sync.Common.Web
{
    /// <summary>Error de negocio esperado: se traduce a un código HTTP concreto.</summary>
    public class ApiException : Exception
    {
        public int StatusCode { get; }
        public ApiException(int statusCode, string message) : base(message) => StatusCode = statusCode;
    }

    public sealed class NotFoundException(string message) : ApiException(StatusCodes.Status404NotFound, message);
    public sealed class BusinessException(string message) : ApiException(StatusCodes.Status400BadRequest, message);
    public sealed class ConflictException(string message) : ApiException(StatusCodes.Status409Conflict, message);
    public sealed class ForbiddenException(string message) : ApiException(StatusCodes.Status403Forbidden, message);

    /// <summary>
    /// Manejador global: convierte excepciones en respuestas ProblemDetails (RFC 7807)
    /// con el código HTTP correcto y registra los errores inesperados.
    /// </summary>
    public sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger, IProblemDetailsService problemDetails) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken ct)
        {
            var (status, title) = exception switch
            {
                ApiException api => (api.StatusCode, api.Message),
                // Errores de negocio lanzados desde SP con THROW 50xxx: 50404 → 404, 50409 → 409, resto → 400
                SqlException { Number: 50404 } sql => (StatusCodes.Status404NotFound, sql.Message),
                SqlException { Number: 50409 } sql => (StatusCodes.Status409Conflict, sql.Message),
                SqlException { Number: >= 50000 } sql => (StatusCodes.Status400BadRequest, sql.Message),
                // 2627/2601 = violación de índice único; 547 = violación de FK/CHECK
                SqlException { Number: 2627 or 2601 } => (StatusCodes.Status409Conflict, "El registro ya existe."),
                SqlException { Number: 547 } => (StatusCodes.Status400BadRequest, "Referencia inválida: el registro relacionado no existe o está en uso."),
                _ => (StatusCodes.Status500InternalServerError, "Error interno del servidor.")
            };

            if (status >= 500)
                logger.LogError(exception, "Error no controlado en {Path}", context.Request.Path);
            else
                logger.LogInformation("Solicitud rechazada {Status} en {Path}: {Message}", status, context.Request.Path, exception.Message);

            context.Response.StatusCode = status;
            return await problemDetails.TryWriteAsync(new ProblemDetailsContext
            {
                HttpContext = context,
                Exception = exception,
                ProblemDetails = new ProblemDetails { Status = status, Title = title }
            });
        }
    }
}
