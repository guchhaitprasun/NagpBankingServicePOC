using System.ComponentModel.DataAnnotations;

namespace AccountService.Data.Entities
{
    public class AccountModel
    {
        [Key]
        public int AccountNumber { get; set; }
        public string AccountHolderName { get; set; }
        public string EmailAddress { get; set; }
        public string AccountType { get; set; }
        public decimal AccountBalance { get; set; }

    }
}
