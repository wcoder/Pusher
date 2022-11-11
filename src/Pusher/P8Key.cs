using System.Security.Cryptography;
using System.Text;

namespace Pusher;

public class P8Key
{
    public string Token { get; }

    private P8Key(string token)
    {
        Token = token;
    }

    public static async Task<P8Key> FromFile(string keyPath, string teamId, string keyId)
    {
        string keyFileData = await File.ReadAllTextAsync(keyPath);
        string rawKeyFileData = keyFileData
            .Replace("-----BEGIN PRIVATE KEY-----", "")
            .Replace("-----END PRIVATE KEY-----", "")
            .Replace("\n", "");
        byte[] privateKeyBytes = Convert.FromBase64String(rawKeyFileData);
 
        // https://stackoverflow.com/a/58861868/5925490
        using ECDsa key = ECDsa.Create();
        key.ImportPkcs8PrivateKey(privateKeyBytes, out _);
            
        string jwtHeader = $"{{\"alg\":\"ES256\",\"kid\":\"{keyId}\"}}";
        string jwtPayload = $"{{\"iss\":\"{teamId}\",\"iat\":{DateTimeOffset.Now.ToUnixTimeSeconds().ToString()}}}";
        
        string headerBase64 = ToBase64Encode(jwtHeader);
        string payloadBase64 = ToBase64Encode(jwtPayload);
        string unsignedJwtData = string
            .Concat(headerBase64, ".", payloadBase64)
            .Replace("+/", "")
            .Replace("-_", "")
            .Replace("=", "");
        byte[] encodedRequest = Encoding.UTF8.GetBytes(unsignedJwtData);

        byte[] signature = key.SignData(encodedRequest, HashAlgorithmName.SHA256);

        string jwt = $"{unsignedJwtData}.{Convert.ToBase64String(signature)}";
        return new P8Key(jwt);
    }

    private static string ToBase64Encode(string plainText)
    {
        var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
        return Convert.ToBase64String(plainTextBytes);
    }
}