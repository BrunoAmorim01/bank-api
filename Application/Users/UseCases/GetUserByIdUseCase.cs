using api.Domain.Repositories;
using api.Shared.Exceptions;

namespace api.Application.Users;
public class GetUserByIdResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }
    public required decimal Balance { get; set; }
}

public class GetUserByIdUseCase(IUserRepository userRepository)
{
    private readonly IUserRepository _userRepository = userRepository;


    public async Task<GetUserByIdResponse> Execute(Guid id)
    {
        var user = await _userRepository.GetById(id) ?? throw new NotFoundException("User not found");

        return new GetUserByIdResponse
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Balance = user.Bank.Balance
        };
    }
}
