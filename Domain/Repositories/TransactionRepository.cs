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

public class UpdateDeposit
{
    public required Guid Id { get; set; }
    public TransactionStatusEnum TransactionStatus { get; set; }
}
public interface ITransactionRepository
{
    Task<TransactionModel> Create(CreateTransaction deposit);
    Task<TransactionModel?> GetById(Guid transactionId);
    Task Update(UpdateDeposit data);

}
