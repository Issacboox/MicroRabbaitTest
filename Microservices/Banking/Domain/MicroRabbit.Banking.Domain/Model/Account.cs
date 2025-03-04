using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MicroRabbit.Banking.Domain.Model
{
    [Table("accounts")]
    public class Account
    {
        [Key]
        [Column("account_id")]
        public int AccountId { get; set; }

        [Column("account_type")]
        public string AccountType { get; set; }

        [Column("account_balance")]
        public decimal AccountBalance { get; set; }
    }
}