using InterventionService.Helpers;
using System.Net;
using System.Text.Json;

namespace InterventionService.Middleware
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
                _logger.LogError(ex, "Une erreur non gérée s'est produite");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = context.Response;
            response.ContentType = "application/json";

            var statusCode = exception switch
            {
                KeyNotFoundException => HttpStatusCode.NotFound,
                ArgumentException => HttpStatusCode.BadRequest,
                UnauthorizedAccessException => HttpStatusCode.Unauthorized,
                ValidationException => HttpStatusCode.BadRequest,
                _ => HttpStatusCode.InternalServerError
            };

            response.StatusCode = (int)statusCode;

            // Message à afficher à l'utilisateur
            string userMessage = statusCode == HttpStatusCode.InternalServerError
                ? "Une erreur interne est survenue"
                : exception.Message;

            // Message de détail pour le logging
            string detailMessage = statusCode == HttpStatusCode.InternalServerError
                ? $"Erreur interne: {exception.Message}"
                : exception.Message;

            // Créer la réponse API
            var apiResponse = new ApiResponse<string>
            {
                Success = false,
                Message = userMessage,
                Data = null,
                Errors = statusCode == HttpStatusCode.BadRequest
                    ? new Dictionary<string, string[]> { { "Server", new[] { detailMessage } } }
                    : null,
                Timestamp = DateTime.UtcNow
            };

            var jsonResponse = JsonSerializer.Serialize(apiResponse, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await response.WriteAsync(jsonResponse);
        }
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }
}