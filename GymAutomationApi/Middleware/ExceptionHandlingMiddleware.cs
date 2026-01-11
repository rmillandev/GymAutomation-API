using GymAutomationApi.Utils.Exceptions;
using System.Net;
using System.Text.Json;

namespace GymAutomationApi.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception occurred: {Message}", ex.Message);
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            httpContext.Response.ContentType = "application/json";
            var statusCode = HttpStatusCode.InternalServerError;
            var message = "An unexpected error occurred.";

            switch (ex)
            {
                case KeyNotFoundException _:
                    statusCode = HttpStatusCode.NotFound;
                    message = ex.Message;
                    break;
                case InvalidOperationException _:
                    statusCode = HttpStatusCode.Conflict;
                    message = ex.Message;
                    break;
                case ResponseParsingException _:
                    statusCode = HttpStatusCode.BadGateway;
                    message = ex.Message;
                    break;
                case BadRequestException _:
                    statusCode = HttpStatusCode.BadRequest;
                    message = ex.Message;
                    break;
                case ExternalServiceException _:
                    statusCode = HttpStatusCode.BadGateway;
                    message = ex.Message;
                    break;

                default:
                    break;
            }

            httpContext.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(new
            {
                statusCode = httpContext.Response.StatusCode,
                errorMessage = message
            });

            return httpContext.Response.WriteAsync(result);
        }
    }
}
