using api.Domain.Repositories;
using api.Domain.Security;
using api.Shared.Exceptions;

namespace api.Application.Users.UseCases;

public class LoginUserResponse
{
    public required object Token { get; set; }
}

public class LoginUserUseCase(IUserRepository userRepository, IAuth authService, IHasher hasher)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IAuth _authService = authService;
    private readonly IHasher _hasher = hasher;

    public async Task<LoginUserResponse> Execute(string email, string password)
    {
        var user = await _userRepository.GetByEmail(email) ?? throw new NotFoundException("User not found");

        string hashedPassword = _hasher.Hash(password);
        
        if (!user.Password.Equals(hashedPassword))
            throw new UnauthorizedException("Invalid Credentials");

        var token = await _authService.GenerateToken(user.Id, user.Name, user.Email);

        return new LoginUserResponse
        {
            Token = token
        };
    }

}

