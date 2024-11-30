using SharedProject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.DTOs
{
    public class TransactionEntryDTO
    {
        public int FromAccountNumber { get; set; }
        public int ToAccountNumber { get; set; }
        public int TransactionForAccountNumber { get; set; }
        public TransactionType TransactionType { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }
        public Guid UniqueTransactionId { get; set; }
    }
}
