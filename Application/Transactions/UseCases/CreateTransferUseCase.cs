using api.Domain.Enums;
using api.Domain.Repositories;
using api.Domain.Services;
using api.Shared.Exceptions;

namespace api.Application.Transactions;

public class AccountTransfer
{
    public required string AccountDigit { get; set; }
    public required string AccountNumber { get; set; }
}
public class TransferData
{
    public decimal Amount { get; set; }
    public required string EmailDestination { get; set; }
    public required AccountTransfer AccountDestination { get; set; }

}

public class CreateTransferUseCase(
    ILogger<CreateDepositUseCase> logger,
    ITransactionRepository transactionRepository,
    IUserRepository userRepository,
    IQueueService queueService)
{

    public async Task<Response> Execute(Guid fromUser, TransferData toAccount)
    {
        logger.LogInformation("Creating transfer");


        var findRequest = new FindRequest
        {
            Email = toAccount.EmailDestination,
            AccountNumber = toAccount.AccountDestination?.AccountNumber,
            AccountDigit = toAccount.AccountDestination?.AccountDigit
        };

        var userDestination = await userRepository.Find(findRequest) ?? throw new NotFoundException("User not found");

        var userOrigin = await userRepository.GetById(fromUser);

        if (userOrigin.Bank.Balance < toAccount.Amount * 100)
        {
            throw new InsufficientBalanceException();
        }

        var transaction = new CreateTransaction
        {
            BankDestinationId = userDestination.Bank.Id,
            BankOriginId = userOrigin.Bank.Id,
            Description = $"Transfer to {userDestination.Name} -  Value: {toAccount.Amount}",
            UserDestinationId = userDestination.Id,
            UserOriginId = userOrigin.Id,
            TransactionStatus = TransactionStatusEnum.Pending,
            TransactionType = TransactionTypeEnum.Transfer,
            Value = (int)(toAccount.Amount * 100),
        };

        var createdTransaction = await transactionRepository.Create(transaction);

        return new Response
        {
            TransactionId = createdTransaction.Id
        };
    }
}

