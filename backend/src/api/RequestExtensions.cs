using Microsoft.IdentityModel.JsonWebTokens;

namespace api;

public static class RequestExtensions
{
    public static string? UserId(this HttpRequest request)
    {
        try
        {
            var accessToken = request.Headers.Authorization.ToString().Split(' ')[1];
            var jsonTokenData = new JsonWebTokenHandler().ReadJsonWebToken(accessToken);
            return jsonTokenData.Subject;
        }
        catch
        {
            return null;
        }
    }

    public static Guid UserId(this HttpContext context)
    {
        return (Guid)context.Items["UserId"]!;
    }
}
