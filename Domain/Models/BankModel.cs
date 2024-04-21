using api.Infrastructure.Database.Entities;

namespace api.Domain.Models;

public class BankModel
{
    public Guid Id { get; set; }
    public required string AccountNumber { get; set; } 
    public required string AccountDigit { get; set; }
    public int Balance { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; } = default!;

}
