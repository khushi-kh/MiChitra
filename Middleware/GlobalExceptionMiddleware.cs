using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace MiChitra.Middleware
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var env = context.RequestServices.GetRequiredService<IHostEnvironment>();

            var response = new
            {
                message = "An error occurred while processing your request.",
                details = env.IsDevelopment() ? exception.Message : null
            };

            context.Response.StatusCode = exception switch
            {
                // Bad format / parsing errors
                FormatException => (int)HttpStatusCode.BadRequest,
                JsonException => (int)HttpStatusCode.BadRequest,
                ArgumentException => (int)HttpStatusCode.BadRequest,

                // Authentication & Authorization
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                ForbiddenAccessException => (int)HttpStatusCode.Forbidden, // custom exception

                // Not Found
                KeyNotFoundException => (int)HttpStatusCode.NotFound,

                // Timeouts
                TimeoutException => (int)HttpStatusCode.RequestTimeout,
                TaskCanceledException => (int)HttpStatusCode.RequestTimeout,

                // Network / External API failures
                HttpRequestException => (int)HttpStatusCode.ServiceUnavailable,

                // Database concurrency (optional but useful)
                DbUpdateConcurrencyException => (int)HttpStatusCode.Conflict,

                // Default
                _ => (int)HttpStatusCode.InternalServerError
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    // Custom exception for Forbidden (403)
    public class ForbiddenAccessException : Exception
    {
        public ForbiddenAccessException(string message = "You do not have permission to access this resource.")
            : base(message)
        {
        }
    }
}
