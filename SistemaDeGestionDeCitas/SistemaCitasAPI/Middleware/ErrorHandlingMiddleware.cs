using System.Net;
using System.Text.Json;

namespace SistemaCitasAPI.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ocurrió un error no controlado");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case KeyNotFoundException:
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new { message = "Recurso no encontrado" });
                    break;
                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonSerializer.Serialize(new { message = "No autorizado" });
                    break;
                case ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new { message = exception.Message });
                    break;
                default:
                    result = JsonSerializer.Serialize(new { message = "Error interno del servidor" });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
