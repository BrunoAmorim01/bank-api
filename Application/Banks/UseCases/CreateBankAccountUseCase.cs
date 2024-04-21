using api.Domain.Repositories;

namespace api.Application.Banks;

public class CreateBankAccountUseCase(ILogger<CreateBankAccountUseCase> logger, IBankRepository bankRepository)
{
    public async Task<BankRegisterResponse> Execute(Guid userId)
    {
        logger.LogInformation("Creating bank account for user...");

        var accountNumber = GenerateAccountNumber();
        var accountDigit = GenerateBankCode();

        var bank = await bankRepository.Create(userId, accountNumber, accountDigit);

        logger.LogInformation("Bank account created successfully");

        return new BankRegisterResponse
        {
            Id = bank.Id,
            AccountNumber = bank.AccountNumber,
            AccountDigit = bank.AccountDigit
        };
    }

    private static string GenerateAccountNumber()
    {
        Random random = new Random();
        string accountNumber = "";
        for (int i = 0; i < 8; i++)
        {
            accountNumber += random.Next(0, 9).ToString();
        }
        return accountNumber;

    }
    private static string GenerateBankCode()
    {
        Random random = new();
        string bankCode = random.Next(0, 9).ToString();
        return bankCode;
    }
}
