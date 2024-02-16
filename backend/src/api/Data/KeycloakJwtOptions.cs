namespace api.Data;
public class KeycloakJwtOptions
{
    public string? Issuer { get; set; }
    public string? Audience { get; set; }
    public string? Secret { get; set; }
}
