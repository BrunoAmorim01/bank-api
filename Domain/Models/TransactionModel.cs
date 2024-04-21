using api.Domain.Enums;
using api.Infrastructure.Database.Entities;

namespace api.Domain.Models;

public class TransactionModel
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UserOriginId { get; set; }
    public Guid UserDestinationId { get; set; }
    public Guid BankOriginId { get; set; }
    public Guid BankDestinationId { get; set; }
    public int Value { get; set; }
    public string Description { get; set; } = default!;
    public TransactionTypeEnum TransactionType { get; set; }
    public TransactionStatusEnum TransactionStatus { get; set; }
    public Bank BankDestination { get; set; } = default!;
    public Bank BankOrigin { get; set; } = default!;
    public User UserOrigin { get; set; } = default!;
    public User UserDestination { get; set; } = default!;

}
