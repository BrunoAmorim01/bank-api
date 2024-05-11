using api.Domain.Repositories;
using api.Infrastructure.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace api.Infrastructure.Database.Repositories;

public class BankRepository(PostgressDbContext dbContext) : IBankRepository
{
    public async Task<Bank> Create(Guid userId, string AccountNumber, string AccountDigit)
    {
        var bank = new Bank
        {
            UserId = userId,
            AccountNumber = AccountNumber,
            AccountDigit = AccountDigit
        };

        var bankTocreate = dbContext.Banks.Add(bank);
        await dbContext.SaveChangesAsync();

        return bankTocreate.Entity;
    }

    public async Task DecrementBalance(UpdateBank data)
    {
        await dbContext.Database.ExecuteSqlRawAsync("UPDATE Banks SET Balance = Balance - {0} WHERE Id = {1}", data.Balance, data.Id);
    }

    public async Task<Bank> GetById(Guid bankId)
    {
        return await dbContext.Banks.FirstAsync(x => x.Id == bankId);
    }

    public async Task IncrementBalance(UpdateBank data)
    {
        await dbContext.Database.ExecuteSqlRawAsync("UPDATE Banks SET Balance = Balance + {0} WHERE Id = {1}", data.Balance, data.Id);

    }
}
