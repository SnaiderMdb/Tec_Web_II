using TecWebII.Api.Core;
using TecWebII.Api.Core.Exceptions;

namespace TecWebII.Api.Core.Middleware;

/// <summary>
/// Middleware global que captura las excepciones y las transforma en respuestas JSON uniformes.
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Error de negocio en {Method} {Path}: {Message}",
                context.Request.Method, context.Request.Path, ex.Message);

            await WriteResponseAsync(context, ex.StatusCode, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error no controlado en {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteResponseAsync(
                context,
                StatusCodes.Status500InternalServerError,
                "Ocurrió un error inesperado en el servidor. Intente nuevamente más tarde.");
        }
    }

    private static async Task WriteResponseAsync(HttpContext context, int statusCode, string message)
    {
        if (context.Response.HasStarted)
        {
            return;
        }

        context.Response.Clear();
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json; charset=utf-8";

        await context.Response.WriteAsJsonAsync(ApiResponse<object>.Fail(message));
    }
}
