using System.Security.Cryptography;
using System.Text;
using api.Domain.Security;

namespace api.Infrastructure.Security;

public class HasherService : IHasher
{
    public string Hash(string text)
    {
        var hashedBytes = SHA256.HashData(Encoding.UTF8.GetBytes(text));
        var hashedString = BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        return hashedString;
    }

    public bool VerifyHash(string text, string hashedText)
    {
        var hashed = Hash(text);
        return hashed.Equals(hashedText);
    }
}
