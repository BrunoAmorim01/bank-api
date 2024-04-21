using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.Infrastructure.Database.Entities
{
    public class Bank : BaseEntity
    {
        public string AccountNumber { get; set; } = default!;
        public string AccountDigit { get; set; } = default!;        
        [DefaultValue(0)]
        public int Balance { get; set; } = default!;        
        [ForeignKey("UserId")]
        [Column("user_id")]
        public Guid UserId { get; set; }                       
        public User User { get; set; } = default!;
    }
}


