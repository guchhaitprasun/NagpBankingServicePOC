using SharedProject.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedProject.DTOs
{
    public class AccountRegistrationDTO
    {
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public AccountEnums AccountType { get; set; }
    }
}
