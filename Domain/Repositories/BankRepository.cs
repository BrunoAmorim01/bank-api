using api.Infrastructure.Database.Entities;

namespace api.Domain.Repositories;

public class UpdateBank
{
    public Guid Id { get; set; }
    public int Balance { get; set; }
}


public interface IBankRepository
{
    Task<Bank> Create(Guid userId, string AccountNumber, string AccountDigit);
    Task Update(UpdateBank data);
    Task<Bank> GetById(Guid bankId);
}



