using api.Domain.Enums;
using api.Domain.Models;

namespace api.Domain.Repositories;

public class CreateTransaction
{
    public Guid UserOriginId { get; set; }
    public Guid UserDestinationId { get; set; }
    public Guid BankOriginId { get; set; }
    public Guid BankDestinationId { get; set; }
    public int Value { get; set; }
    public string? Description { get; set; }
    public TransactionTypeEnum TransactionType { get; set; }
    public TransactionStatusEnum TransactionStatus { get; set; }

}

public class UpdateTransaction
{
    public required Guid Id { get; set; }
    public TransactionStatusEnum TransactionStatus { get; set; }
}

public class FindQuery
{
    public DateOnly? StartDate { get; set; }
    public DateOnly? EndDate { get; set; }
    public TransactionTypeEnum[]? TransactionType { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 10;

}
public interface ITransactionRepository
{
    Task<TransactionModel> Create(CreateTransaction deposit);
    Task<TransactionModel?> GetById(Guid transactionId);
    Task Update(UpdateTransaction data);
    Task<TransactionModel[]> ListByUserId(Guid userId, FindQuery query);

}
