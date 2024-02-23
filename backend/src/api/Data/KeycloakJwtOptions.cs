using System.Diagnostics.CodeAnalysis;

namespace api.Data;
public class KeycloakJwtOptions
{
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public required string Secret { get; set; }

    [SetsRequiredMembers]
    public KeycloakJwtOptions(string issuer, string audience, string secret)
    {
        Issuer = issuer;
        Audience = audience;
        Secret = secret;
    }
}
