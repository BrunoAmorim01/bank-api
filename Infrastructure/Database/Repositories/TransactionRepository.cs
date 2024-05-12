using api.Domain.Models;
using api.Domain.Repositories;
using api.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Database.Repositories;

public class TransactionRepository(PostgressDbContext dbContext) : ITransactionRepository
{
    public async Task<TransactionModel> Create(CreateTransaction deposit)
    {
        var transaction = new Transaction
        {
            UserOriginId = deposit.UserOriginId,
            UserDestinationId = deposit.UserDestinationId,
            BankOriginId = deposit.BankOriginId,
            BankDestinationId = deposit.BankDestinationId,
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
        var transaction = await dbContext.Transactions
        .Include(x => x.UserDestination)
        .Include(x => x.UserOrigin)
        .Select(x => new TransactionModel
        {
            Id = x.Id,
            UserDestinationId = x.UserDestinationId,
            BankDestinationId = x.BankDestinationId,
            BankOriginId = x.BankOriginId,
            UserOriginId = x.UserOriginId,
            TransactionStatus = x.TransactionStatus,
            TransactionType = x.TransactionType,
            Value = x.Value,
            UserDestination = new User
            {
                Id = x.UserDestination.Id,
                Name = x.UserDestination.Name,
                Email = x.UserDestination.Email
            },
            UserOrigin = new User
            {
                Id = x.UserOrigin.Id,
                Name = x.UserOrigin.Name,
                Email = x.UserOrigin.Email
            }

        }).FirstOrDefaultAsync(x => x.Id == transactionId);

        if (transaction is null)
        {
            return null;
        }

        return transaction;
    }

    public async Task<TransactionModel[]> ListByUserId(Guid userId)
    {
        return await dbContext.Transactions
        .Include(x => x.UserDestination)
        .Include(x => x.UserOrigin)
        .Include(x => x.BankDestination)
        .Include(x => x.BankOrigin)
        .Select(x => new TransactionModel
        {
            Id = x.Id,            
            TransactionStatus = x.TransactionStatus,
            TransactionType = x.TransactionType,
            Value = x.Value,
            CreatedAt = x.CreatedAt,
            UserDestination = new User
            {
                Id = x.UserDestination.Id,
                Name = x.UserDestination.Name,
                Email = x.UserDestination.Email,

             },
            UserOrigin = new User
            {
                Id = x.UserOrigin.Id,
                Name = x.UserOrigin.Name,
                Email = x.UserOrigin.Email
            },
            BankDestination = new Bank
            {
                Id = x.BankDestination.Id,
                AccountDigit = x.BankDestination.AccountDigit,
                AccountNumber = x.BankDestination.AccountNumber,
            },
            BankOrigin = new Bank
            {
                Id = x.BankOrigin.Id,
                AccountDigit = x.BankOrigin.AccountDigit,
                AccountNumber = x.BankOrigin.AccountNumber,
            }

        }).Where(x => x.UserDestination.Id.Equals (userId) || x.UserDestination.Id.Equals (userId)).ToArrayAsync();
    }

    public async Task Update(UpdateTransaction data)
    {
        await dbContext.Transactions.Where(x => x.Id == data.Id).ExecuteUpdateAsync(x => x.SetProperty(p => p.TransactionStatus, data.TransactionStatus));
    }
}
