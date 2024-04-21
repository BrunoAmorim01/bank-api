using System.ComponentModel.DataAnnotations.Schema;

namespace api.Infrastructure.Database.Entities;

public class User : BaseEntity
{
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Password { get; set; } = default!;
    public Bank Bank { get; set; } = default!;
    //[ForeignKey("UserDestinationId")]
    public ICollection<Transaction> TransactionsDestination { get; set; } = [];
    //[ForeignKey("UserOriginId")]
    public ICollection<Transaction> TransactionsOrigin { get; set; } = [];

}
