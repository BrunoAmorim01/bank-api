using api.Domain.Enums;
using api.Domain.Repositories;

namespace api.Application.Transactions;

public class ProccessDepositUseCase(ILogger<ProccessDepositUseCase> logger, ITransactionRepository transactionRepository, IBankRepository bankRepository)
{
    public async Task Execute(Guid transactionId)
    {
        logger.LogInformation("Processing deposit");

        var transaction = await transactionRepository.GetById(transactionId);
        Console.WriteLine("Transaction: ");
        Console.WriteLine(transaction.TransactionStatus);

        if (transaction?.TransactionStatus != TransactionStatusEnum.Pending || transaction is null)
        {
            throw new Exception("Transaction is not pending");
        }
        logger.LogInformation("Updating transaction status");
        var updateDeposit = new UpdateDeposit
        {
            Id = transactionId,
            TransactionStatus = TransactionStatusEnum.Completed
        };

        await transactionRepository.Update(updateDeposit);

        logger.LogInformation("Updating bank balance");

        var bank = await bankRepository.GetById(transaction.BankDestinationId);

        logger.LogInformation("Bank balance: {0}", bank.Balance);
        logger.LogInformation("Transaction value: {0}", transaction.Value);

        var bankUpdate = new UpdateBank
        {
            Id = transaction.BankDestinationId,
            Balance = transaction.Value + bank.Balance
        };

        await bankRepository.Update(bankUpdate);

        logger.LogInformation("Deposit processed");

    }

}
