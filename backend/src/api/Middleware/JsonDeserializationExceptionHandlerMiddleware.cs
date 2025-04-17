using System.Text.Json;

namespace api.Middleware;

public class JsonDeserializationExceptionHandlerMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (BadHttpRequestException ex)
        {
            if (ex.InnerException is JsonException)
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;

                var cause = (ex.InnerException?.Message.Split(Environment.NewLine)[0] ?? string.Empty).AsSpan();
                var start = cause.IndexOf("missing");
                if (start != -1){
                    cause = cause[start..];
                }

                if (cause.Length > 100)
                {
                    cause = string.Concat(cause[..100], "...");
                }

                await context.Response.WriteAsJsonAsync(new 
                {
                    error = "Invalid JSON in request body",
                    details = cause.ToString()
                });
            }
            else
            {
                throw;
            }
        }
    }
}