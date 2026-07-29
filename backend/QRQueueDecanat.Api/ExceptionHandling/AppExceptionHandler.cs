using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.Exceptions;

namespace QRQueueDecanat.ExceptionHandling;

public class AppExceptionHandler : IExceptionHandler
{
    private readonly ILogger<AppExceptionHandler> _logger;

    public AppExceptionHandler(ILogger<AppExceptionHandler> logger)
    {
        _logger = logger;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext,
        Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            InvalidCredentialsException => (
                StatusCodes.Status401Unauthorized,
                "Ошибка авторизации",
                exception.Message
            ),
            NotFoundException => (
                StatusCodes.Status404NotFound,
                "Ресурс не найден",
                exception.Message
            ),
            ConflictException => (
                StatusCodes.Status409Conflict,
                "Действие недоступно",
                exception.Message
            ),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Внутренняя ошибка сервера",
                "Не удалось выполнить запрос."
            )
        };
        if (statusCode >= 500)
        {
            _logger.LogError(exception, "Path: {Path}. Message: {Message}",
                httpContext.Request.Path, exception.Message);
        }
        else
        {
            _logger.LogWarning("Path: {Path}. Message: {Message}",
                httpContext.Request.Path, exception.Message);
        }
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };
        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";
        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}