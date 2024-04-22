using api.Application.Emails;
using api.Domain.Enums;
using api.Domain.Repositories;
using api.Domain.Services;

namespace api.Application.Transactions;

public class ProccessDepositUseCase(
    ILogger<ProccessDepositUseCase> logger,
    ITransactionRepository transactionRepository,
    IBankRepository bankRepository,
    SendCompletedDepositEmailUseCase sendCompletedDepositEmailUseCase
    )
{
    public async Task Execute(Guid transactionId)
    {
        logger.LogInformation("Processing deposit");

        var transaction = await transactionRepository.GetById(transactionId);

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

        logger.LogInformation("Sending email");
        await sendCompletedDepositEmailUseCase.Execute(transaction.UserDestination.Email, transaction.UserDestination.Name, transaction.Value);

        logger.LogInformation("Deposit processed");

    }

}
