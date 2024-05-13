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

    public async Task<TransactionModel[]> ListByUserId(Guid userId, FindQuery query)
    {
        var queryContext = from transaction in dbContext.Transactions
                           join userDestination in dbContext.Users on transaction.UserDestinationId equals userDestination.Id into _userDestination
                           from userDestination in _userDestination.DefaultIfEmpty()
                           join bankDestination in dbContext.Banks on transaction.BankDestinationId equals bankDestination.Id into _bankDestination
                           from bankDestination in _bankDestination.DefaultIfEmpty()
                           join userOrigin in dbContext.Users on transaction.UserOriginId equals userOrigin.Id into _userOrigin
                           from userOrigin in _userOrigin.DefaultIfEmpty()
                           join bankOrigin in dbContext.Banks on transaction.BankOriginId equals bankOrigin.Id into _bankOrigin
                           from bankOrigin in _bankOrigin.DefaultIfEmpty()
                           where transaction.UserDestinationId == userId || transaction.UserOriginId == userId
                           select new TransactionModel
                           {
                               Id = transaction.Id,
                               CreatedAt = transaction.CreatedAt,
                               Value = transaction.Value,
                               TransactionType = transaction.TransactionType,
                               TransactionStatus = transaction.TransactionStatus,
                               UserDestination = new User
                               {
                                   Id = userDestination.Id,
                                   Name = userDestination.Name,
                                   Email = userDestination.Email
                               },
                               BankDestination = new Bank
                               {
                                   Id = bankDestination.Id,
                                   AccountDigit = bankDestination.AccountDigit,
                                   AccountNumber = bankDestination.AccountNumber
                               },
                               UserOrigin = new User
                               {
                                   Id = userOrigin.Id,
                                   Name = userOrigin.Name,
                                   Email = userOrigin.Email
                               },
                               BankOrigin = new Bank
                               {
                                   Id = bankOrigin.Id,
                                   AccountDigit = bankOrigin.AccountDigit,
                                   AccountNumber = bankOrigin.AccountNumber
                               }
                           };

        if (query.StartDate is not null && query.EndDate is not null)
        {
            DateTime startDate = new(query.StartDate.Value.Year, query.StartDate.Value.Month, query.StartDate.Value.Day, 0, 0, 0, DateTimeKind.Utc);
            DateTime endDate = new(query.EndDate.Value.Year, query.EndDate.Value.Month, query.EndDate.Value.Day, 23, 59, 59, DateTimeKind.Utc);
            queryContext = queryContext.Where(y => y.CreatedAt >= startDate && y.CreatedAt <= endDate);
        }

        if (query.TransactionType?.Length > 0)
        {
            queryContext = queryContext.Where(x => query.TransactionType.Contains(x.TransactionType));
        }

        if (query.TransactionStatus?.Length > 0)
        {
            queryContext = queryContext.Where(x => query.TransactionStatus.Contains(x.TransactionStatus));
        }
       
        queryContext = queryContext.Skip(query.Skip).Take(query.Take);
        
        return await queryContext.ToArrayAsync();
    }

    public async Task Update(UpdateTransaction data)
    {
        await dbContext.Transactions.Where(x => x.Id == data.Id).ExecuteUpdateAsync(x => x.SetProperty(p => p.TransactionStatus, data.TransactionStatus));
    }
}
