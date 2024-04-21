using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using api.Domain.Security;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace api.Infrastructure.Security;

public class AuthService(IOptions<JwtSettings> jwtSettings) : IAuth
{
    private readonly JwtSettings _jwsSettings = jwtSettings.Value;
    public Task<string> GenerateToken(Guid userId, string name, string email)
    {

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwsSettings.Key);
        var tokenDescriptor = new SecurityTokenDescriptor
        {

            Expires = DateTime.UtcNow.AddDays(7),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            Issuer = _jwsSettings.Issuer,
            Audience = _jwsSettings.Audience,
           Subject = new ClaimsIdentity(
            [
                //new Claim("UserId", userId.ToString()),
                new Claim(ClaimTypes.Name, name),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.UserData, userId.ToString())
            ])
        };
        var token = tokenHandler.CreateToken(tokenDescriptor);
        var tokenString = tokenHandler.WriteToken(token);
        Console.WriteLine(tokenString);
        return Task.FromResult(tokenString);
    }
}
