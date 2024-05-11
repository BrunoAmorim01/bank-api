
using api.Application.Emails;
using api.Domain.Enums;
using api.Domain.Models;
using api.Domain.Repositories;


namespace api.Application.Transactions;

public class ProccessTransferUseCase(
    ILogger<ProccessTransferUseCase> logger,
    ITransactionRepository transactionRepository,
    IBankRepository bankRepository,
    SendCompletedTransferDestinationEmailUseCase sendCompletedTransferDestinationEmailUseCase,
    SendCompletedTransferOriginEmailUseCase sendCompletedTransferOriginEmailUseCase
    )
{
    public async Task Execute(Guid transactionId)
    {
        logger.LogInformation("Processing transfer");

        var transfer = await transactionRepository.GetById(transactionId);

        if (transfer?.TransactionStatus != TransactionStatusEnum.Pending || transfer is null)
        {
            logger.LogInformation($"Transfer {transactionId} not found or already processed");
            return;
        }

        logger.LogInformation("Updating transfer status");
        var updateTransfer = new UpdateTransaction
        {
            Id = transactionId,
            TransactionStatus = TransactionStatusEnum.Completed,
        };

        await transactionRepository.Update(updateTransfer);

        await ProcessDestination(transfer);
        await ProcessOrigin(transfer);
        
        logger.LogInformation("Transfer processed");

    }

    private async Task ProcessDestination(TransactionModel transfer)
    {
        logger.LogInformation("Updating bank destination balance");

        var bankDestination = await bankRepository.GetById(transfer.BankDestinationId);

        logger.LogInformation("Bank destination balance: {0}", bankDestination.Balance);
        logger.LogInformation("Transaction destination value: {0}", transfer.Value);

        var bankDestinationUpdate = new UpdateBank
        {
            Id = transfer.BankDestinationId,
            Balance = transfer.Value
        };

        await bankRepository.IncrementBalance(bankDestinationUpdate);

        logger.LogInformation("Sending destination email");
        await sendCompletedTransferDestinationEmailUseCase.Execute(transfer.UserDestination.Email, transfer.UserDestination.Name, transfer.Value);
    }

    private async Task ProcessOrigin(TransactionModel transfer)
    {
        logger.LogInformation("Updating bank origin balance");

        var bankOrigin = await bankRepository.GetById(transfer.BankOriginId);

        logger.LogInformation("Bank origin balance: {0}", bankOrigin.Balance);
        logger.LogInformation("Transaction origin value: {0}", transfer.Value);

        var bankOriginUpdate = new UpdateBank
        {
            Id = transfer.BankOriginId,
            Balance = transfer.Value
        };

        await bankRepository.DecrementBalance(bankOriginUpdate);

        logger.LogInformation("Sending origin email");
        await sendCompletedTransferOriginEmailUseCase.Execute(transfer.UserOrigin.Email, transfer.UserOrigin.Name, transfer.Value);
    }
}
