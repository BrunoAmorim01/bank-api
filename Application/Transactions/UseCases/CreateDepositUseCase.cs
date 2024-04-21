using System.Text.Json;
using api.Domain.Enums;
using api.Domain.Repositories;
using api.Domain.Services;

namespace api.Application.Transactions;

public class Response
{
    public Guid TransactionId { get; set; }
}
public class CreateDepositUseCase(ILogger<CreateDepositUseCase> logger, ITransactionRepository transactionRepository, IUserRepository userRepository, IQueueService queueService)
{

    public async Task<Response> Execute(Guid userId, decimal amount)
    {
        logger.LogInformation("Creating deposit");

        var user = await userRepository.GetById(userId);

        var deposit = new CreateDeposit
        {
            UserId = userId,
            BankId = user.Bank.Id,
            Value = Convert.ToInt32(Math.Round(amount * 100)),
            TransactionType = TransactionTypeEnum.Deposit,
            TransactionStatus = TransactionStatusEnum.Pending,
        };

        var transactionCreated = await transactionRepository.Create(deposit);

        logger.LogInformation("Deposit created");

        logger.LogInformation("Publishing message to queue");

        var response = new Response
        {
            TransactionId = transactionCreated.Id
        };

        var message = JsonSerializer.Serialize(response);

        await queueService.PublishDepositAsync(message, transactionCreated.Id.ToString());
        return response;
    }


}

