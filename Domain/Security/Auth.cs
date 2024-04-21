namespace api.Domain.Security;

public interface IAuth
{
    public Task<string> GenerateToken(Guid userId, string name, string email);
}

