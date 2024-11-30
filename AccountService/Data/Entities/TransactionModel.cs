using System.ComponentModel.DataAnnotations;

namespace AccountService.Data.Entities
{
    public class TransactionModel
    {
        [Key]
        public int TransactionId { get; set; }
        public Guid ReferenceNumber { get; set; }
        public string TransactionType { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }
        public DateTime DateTime { get; set; }


        // foreign Key
        public int FromAccountId { get; set; }
        public AccountModel FromAccount { get; set; }
        public int ToAccountId { get; set; }
        public AccountModel ToAccount { get; set; }
        public int TransactionForAccountId { get; set; }
        public AccountModel TransactionForAccount { get; set; }
    }
}
