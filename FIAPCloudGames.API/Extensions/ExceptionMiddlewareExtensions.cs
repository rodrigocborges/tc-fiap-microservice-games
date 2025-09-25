using FIAPCloudGames.API.Middlewares;

namespace FIAPCloudGames.API.Extensions
{
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
        {
            return app.UseMiddleware<GenericExceptionHandlerMiddleware>();
        }
    }
}
