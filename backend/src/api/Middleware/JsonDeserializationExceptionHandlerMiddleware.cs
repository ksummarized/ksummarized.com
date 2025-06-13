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

                string causeString = ex.InnerException?.Message.Split(Environment.NewLine)[0] ?? string.Empty;
                var start = causeString.IndexOf("missing");
                if (start != -1)
                {
                    causeString = causeString[start..];
                }

                if (causeString.Length > 100)
                {
                    // Ensure Substring arguments are valid
                    causeString = causeString.Substring(0, Math.Min(causeString.Length, 100)) + "...";
                }

                await context.Response.WriteAsJsonAsync(new 
                {
                    error = "Invalid JSON in request body",
                    details = causeString
                });
            }
            else
            {
                throw;
            }
        }
    }
}