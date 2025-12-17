using Explorer.BuildingBlocks.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace Explorer.API.Middleware;

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
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            ArgumentException ex => (HttpStatusCode.BadRequest, ex.Message),
            // map UnauthorizedAccessException to 403 Forbidden (authenticated but not allowed)
            UnauthorizedAccessException ex => (HttpStatusCode.Forbidden, ex.Message),
            // domain-specific forbidden
            ForbiddenException ex => (HttpStatusCode.Forbidden, ex.Message),
            NotFoundException ex => (HttpStatusCode.NotFound, ex.Message),
            EntityValidationException ex => (HttpStatusCode.UnprocessableEntity, ex.Message),
            // map invalid operations (domain state conflicts) to 409 Conflict
            InvalidOperationException ex => (HttpStatusCode.Conflict, ex.Message),
            _ => (HttpStatusCode.InternalServerError, "An internal server error occurred.")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            title = "An error occurred",
            status = (int)statusCode,
            detail = message
        };

        var jsonResponse = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(jsonResponse);
    }
}