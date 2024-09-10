namespace infrastructure.Keycloak;
public class KeycloakJwtOptions
{
    public required string Issuer { get; init; }
    public required string Audience { get; init; }
    public required string Secret { get; init; }
}
