

using System.ComponentModel.DataAnnotations;
using api.Domain.Enums;

namespace api.Infrastructure.Database.Entities
{
    public class Transaction : BaseEntity
    {
        public Guid UserOriginId { get; set; }
        public Guid UserDestinationId { get; set; }
        public Guid BankOriginId { get; set; }
        public Guid BankDestinationId { get; set; }
        public int Value { get; set; }
        public string? Description { get; set; }
        [EnumDataType(typeof(TransactionTypeEnum))]
        public TransactionTypeEnum TransactionType { get; set; }
        [EnumDataType(typeof(TransactionStatusEnum))]
        public TransactionStatusEnum TransactionStatus { get; set; }
        //[ForeignKey("BankDestinationId")]
        public Bank BankDestination { get; set; } = default!;
        //[ForeignKey("BankOriginId")]      
        public Bank BankOrigin { get; set; } = default!;
        //[ForeignKey("UserOriginId")]
        public User UserOrigin { get; set; } = default!;
        //[ForeignKey("UserDestinationId")]
        public User UserDestination { get; set; } = default!;

    }
}

