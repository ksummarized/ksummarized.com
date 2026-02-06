using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;

namespace Ksummarized.IntegrationTests;

public static class TestJwtToken
{
    private static readonly string _publicKey;

    static TestJwtToken()
    {
        using var rsa = RSA.Create(2048);
        _publicKey = Convert.ToBase64String(rsa.ExportSubjectPublicKeyInfo());
    }

    public static string PublicKey => _publicKey;

    public static string Create(Guid userId)
    {
        var header = Base64UrlEncoder.Encode(JsonSerializer.Serialize(new { alg = "none", typ = "JWT" }));
        var payload = Base64UrlEncoder.Encode(JsonSerializer.Serialize(new { sub = userId.ToString() }));
        return $"{header}.{payload}.";
    }
}
