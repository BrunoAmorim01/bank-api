using api.Domain.Models;
using api.Domain.Repositories;
using api.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Database.Repositories;

public class TransactionRepository(PostgressDbContext dbContext) : ITransactionRepository
{


    public async Task<TransactionModel> Create(CreateDeposit deposit)
    {
        var transaction = new Transaction
        {
            UserOriginId = deposit.UserId,
            UserDestinationId = deposit.UserId,
            BankOriginId = deposit.BankId,
            BankDestinationId = deposit.BankId,
            Value = deposit.Value,
            Description = deposit.Description,
            TransactionType = deposit.TransactionType,
            TransactionStatus = deposit.TransactionStatus
        };

        var transactionToCreate = dbContext.Transactions.Add(transaction);
        await dbContext.SaveChangesAsync();

        var transactionModel = new TransactionModel
        {
            Id = transactionToCreate.Entity.Id,
        };

        return transactionModel;

    }

    public async Task<TransactionModel?> GetById(Guid transactionId)
    {
        var transaction = await dbContext.Transactions.Select(x => new TransactionModel
        {
            Id = x.Id,
            UserDestinationId = x.UserDestinationId,
            BankDestinationId = x.BankDestinationId,
            TransactionStatus = x.TransactionStatus,
            TransactionType = x.TransactionType,
            Value = x.Value

        }).FirstOrDefaultAsync(x => x.Id == transactionId);

        if (transaction is null)
        {
            return null;
        }


        var transactionModel = new TransactionModel
        {
            Id = transaction.Id,
            UserDestinationId = transaction.UserDestinationId,
            BankDestinationId = transaction.BankDestinationId,
            TransactionStatus = transaction.TransactionStatus,
            TransactionType = transaction.TransactionType,
            Value = transaction.Value
        };


        return transactionModel;
    }

    public async Task Update(UpdateDeposit data)
    {
        await dbContext.Transactions.Where(x => x.Id == data.Id).ExecuteUpdateAsync(x => x.SetProperty (p => p.TransactionStatus, data.TransactionStatus));
    }
}
