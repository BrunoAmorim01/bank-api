

namespace api.Domain.Security;

public interface IHasher
{
    string Hash(String text);
    bool VerifyHash(String text, String hashedText);
}
