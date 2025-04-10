namespace api.Middleware;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseExceptionHandlers(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<JsonDeserializationExceptionHandlerMiddleware>();
    }
}