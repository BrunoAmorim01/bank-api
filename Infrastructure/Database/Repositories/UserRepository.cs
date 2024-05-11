using Microsoft.EntityFrameworkCore;
using api.Domain.Models;
using api.Domain.Repositories;
using api.Infrastructure.Database.Entities;

namespace api.Infrastructure.Database.Repositories;

public class UserRepository(PostgressDbContext dbContext) : IUserRepository
{
    public async Task<User> Create(UserModel user)
    {
        var userModel = new User
        {
            Name = user.Name,
            Password = user.Password,
            Email = user.Email
        };

        var userToCreate = dbContext.Users.Add(userModel);
        await dbContext.SaveChangesAsync();
        return userToCreate.Entity;

    }

    public Task<User?> Find(FindRequest request)
    {
        var query = dbContext.Users.Include(u => u.Bank).AsQueryable();

        if (!string.IsNullOrEmpty(request.Email))
        {
            query = query.Where(u => u.Email.Equals(request.Email));
        }

        if (!string.IsNullOrEmpty(request.AccountNumber) && !string.IsNullOrEmpty(request.AccountDigit))
        {
            query = query.Where(u => u.Bank.AccountNumber.Equals(request.AccountNumber) && u.Bank.AccountDigit.Equals(request.AccountDigit));
        }

        return query.Select(u => new User { Id = u.Id, Bank = new Bank { Id = u.Bank.Id } }).FirstOrDefaultAsync();
    }

    public Task<User?> GetByEmail(string email)
    {
        return dbContext.Users
            .Where(u => u.Email.Equals(email))
            .Select(u => new User { Name = u.Name, Password = u.Password, Email = u.Email, Id = u.Id })
            .FirstOrDefaultAsync();
    }

    public Task<User> GetById(Guid id)
    {
        return dbContext.Users
            .Where(u => u.Id.Equals(id))
            .Include(u => u.Bank)
            .Select(u => new User { Name = u.Name, Email = u.Email, Id = u.Id, Bank = new Bank { Id = u.Bank.Id, Balance = u.Bank.Balance } })
            .FirstAsync();
    }
}
