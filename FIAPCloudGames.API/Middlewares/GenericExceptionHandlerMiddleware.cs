using System.Net;
using System.Text.Json;

namespace FIAPCloudGames.API.Middlewares
{
    public class GenericExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GenericExceptionHandlerMiddleware> _logger;

        public GenericExceptionHandlerMiddleware(RequestDelegate next, ILogger<GenericExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context); // chama o próximo middleware
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred while processing the request.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    error = "An unexpected error occurred.",
                    detail = ex.Message,
                    traceId = context.TraceIdentifier
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
            }
        }
    }
}
