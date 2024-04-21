using System.Security.Cryptography;
using System.Text;
using api.Application.Banks;
using api.Application.Emails;
using api.Domain.Models;
using api.Domain.Repositories;
using api.Domain.Security;
using api.Shared.Exceptions;


namespace api.Application.Users;

public class Response
{
    public Guid Id { get; set; }
}

public class RegisterUserUseCase(
    ILogger<RegisterUserUseCase> logger,
    IUserRepository userRepository,
    CreateBankAccountUseCase createBankAccountUseCase,
    SendWelcomeEmailUseCase sendWelcomeEmailUseCase,
    IHasher hasher
    )
{

    public async Task<Response> Execute(string name, string password, string email)
    {
        var userExists = await userRepository.GetByEmail(email);
        if (userExists != null)
        {
            throw new AlreadyExistsException("User already exists");
        }


        logger.LogInformation("Registering user...");
        var userCreated = await userRepository.Create(new UserModel
        {
            Name = name,
            Password = hasher.Hash(password),
            Email = email
        });
        logger.LogInformation("User registered successfully");

        logger.LogInformation("Create bank account for user...");
        var bankCreated = await createBankAccountUseCase.Execute(userCreated.Id);
        logger.LogInformation("Bank account created successfully");

        logger.LogInformation("Sending welcome email...");
        await sendWelcomeEmailUseCase.Execute(userCreated.Email, bankCreated.AccountNumber, bankCreated.AccountDigit, userCreated.Name);
        logger.LogInformation("Welcome email sent successfully");

        var response = new Response
        {
            Id = userCreated.Id
        };
        return response;
    }

}



