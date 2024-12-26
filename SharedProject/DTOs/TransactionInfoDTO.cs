using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.DTOs
{
    public class TransactionInfoDTO
    {
        public int AccountNumber { get; set; }
        public string TransactionType { get; set; }
        public decimal Amount { get; set; }
        public string Remark { get; set; }
        public DateTime TransactionDate { get; set; }
    }
}
