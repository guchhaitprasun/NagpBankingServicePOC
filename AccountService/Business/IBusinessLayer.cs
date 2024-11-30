using AccountService.Data.Entities;
using SharedProject.DTOs;

namespace AccountService.Business
{
    public interface IBusinessLayer
    {
        ResponseModel CreateNewAccount(AccountRegistrationDTO accountRegistrationDTO);
        ResponseModel Addfunds(AddfundsDTO addfundsDTO);
        ResponseModel Sendmoney(SendMoneyDTO sendMoneyDTO);
        ResponseModel AccountStatement(AccountStatementRequestDTO accountStatementRequestDTO);
    }
}
