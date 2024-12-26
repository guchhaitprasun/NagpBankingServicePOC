using AccountService.Data;
using AccountService.Data.Entities;
using Azure.Core;
using SharedProject.DTOs;
using SharedProject.Enums;
using System;

namespace AccountService.Business
{
    public class BusinessLayer : IBusinessLayer
    {
        private readonly ApplicationDbContext _context;

        public BusinessLayer(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Business Logic to create new account
        /// </summary>
        /// <param name="accountRegistrationDTO">Account Registration Object with required properties to create account</param>
        /// <returns>Response Model containing status of the function call</returns>
        public ResponseModel CreateNewAccount(AccountRegistrationDTO accountRegistrationDTO)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                AccountModel accountModel = new AccountModel
                {
                    AccountType = accountRegistrationDTO.AccountType.ToString().ToLower() == "current" ? AccountEnums.Savings.ToString() : AccountEnums.Savings.ToString(),
                    AccountHolderName = accountRegistrationDTO.FullName,
                    EmailAddress = accountRegistrationDTO.EmailAddress,
                    AccountBalance = 0
                };

                _context.Accounts.Add(accountModel);
                _context.SaveChanges();

                responseModel.Status = true;
                responseModel.Message = "Account Created successfully";
                responseModel.Data = accountModel;
            }
            catch (Exception ex)
            {
                responseModel.Status = false;
                responseModel.Message = "Failed to creat new account";
                responseModel.Data = ex.Message;
            }

            return responseModel;
        }

        /// <summary>
        /// Business Logic to add funds to the existing account
        /// </summary>
        /// <param name="addfundsDTO"></param>
        /// <returns></returns>
        public ResponseModel Addfunds(AddfundsDTO addfundsDTO)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                //Fetch Account holder
                var holder = _context.Accounts.Where(o => o.AccountNumber == addfundsDTO.AccountNumber).FirstOrDefault();
                if (holder != null)
                {
                    holder.AccountBalance += addfundsDTO.Amount;

                    _context.Accounts.Update(holder);
                    _context.SaveChanges();

                    Guid transactionId = Guid.NewGuid();
                    AddNewTransactionEntry(new TransactionEntryDTO
                    {
                        FromAccountNumber = addfundsDTO.AccountNumber,
                        ToAccountNumber = addfundsDTO.AccountNumber,
                        TransactionForAccountNumber = addfundsDTO.AccountNumber,
                        Amount = addfundsDTO.Amount,
                        TransactionType = TransactionType.Credit,
                        Remark = "funds deposit to the account",
                        UniqueTransactionId = transactionId,
                    });

                    responseModel.Status = true;
                    responseModel.Message = $"Funds Added Successfully Transaction Reference {transactionId}";
                    responseModel.Data = addfundsDTO;
                }
            }
            catch (Exception ex)
            {
                responseModel.Status = false;
                responseModel.Message = "Error occured, please contact system admin";
                responseModel.Data = ex.Message;
            }

            return responseModel;
        }

        /// <summary>
        /// Business Logic to Send Money to another account
        /// </summary>
        /// <param name="sendMoneyDTO"></param>
        /// <returns></returns>
        public ResponseModel Sendmoney(SendMoneyDTO sendMoneyDTO)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                //Fetch Account Information of the receipient
                AccountModel? Sender = _context.Accounts.Where(o => o.AccountNumber == sendMoneyDTO.FromAccount).FirstOrDefault();
                AccountModel? Receiver = _context.Accounts.Where(o => o.AccountNumber == sendMoneyDTO.ToAccount).FirstOrDefault();

                if (Sender != null && Receiver != null)
                {
                    if (Sender.AccountBalance >= sendMoneyDTO.Amount)
                    {
                        Guid uniqueTransactionReference = Guid.NewGuid();

                        Sender.AccountBalance -= sendMoneyDTO.Amount;
                        _context.Accounts.Update(Sender);

                        AddNewTransactionEntry(new TransactionEntryDTO
                        {
                            FromAccountNumber = sendMoneyDTO.FromAccount,
                            ToAccountNumber = sendMoneyDTO.ToAccount,
                            TransactionForAccountNumber = Sender.AccountNumber,
                            Amount = sendMoneyDTO.Amount,
                            TransactionType = TransactionType.Debit,
                            Remark = $"Rs.{sendMoneyDTO.Amount} is debited from account number {Sender.AccountNumber}, available balance: Rs.{Sender.AccountBalance - sendMoneyDTO.Amount}, Remark: {sendMoneyDTO.Note}",
                            UniqueTransactionId = uniqueTransactionReference,
                        });

                        Receiver.AccountBalance += sendMoneyDTO.Amount;
                        _context.Accounts.Update(Receiver);

                        AddNewTransactionEntry(new TransactionEntryDTO
                        {
                            FromAccountNumber = sendMoneyDTO.FromAccount,
                            ToAccountNumber = sendMoneyDTO.ToAccount,
                            TransactionForAccountNumber = Receiver.AccountNumber,
                            Amount = sendMoneyDTO.Amount,
                            TransactionType = TransactionType.Credit,
                            Remark = $"Rs.{sendMoneyDTO.Amount} Received from {Sender.AccountHolderName}, Account Number: {Sender.AccountNumber}",
                            UniqueTransactionId = uniqueTransactionReference,
                        });

                        responseModel.Status = true;
                        responseModel.Message = "Transaction Completed Successfully";
                        responseModel.Data = uniqueTransactionReference;
                    }
                }
                else
                {
                    responseModel.Status = false;
                    responseModel.Message = "Please provide the correct account information";
                    responseModel.Data = new { };
                }

            }
            catch (Exception ex)
            {
                responseModel.Status = false;
                responseModel.Message = "Error occured contact system admin";
                responseModel.Data = ex.Message;
            }

            return responseModel;
        }

        /// <summary>
        /// Get account statement
        /// </summary>
        /// <param name="sendMoneyDTO"></param>
        /// <returns></returns>
        public ResponseModel AccountStatement(AccountStatementRequestDTO accountStatementRequestDTO)
        {
            ResponseModel responseModel = new ResponseModel();

            try
            {
                var transactions = _context
                                    .Transactions
                                    .Where(o => o.TransactionForAccountId == accountStatementRequestDTO.AccountNumber
                                    && o.DateTime >= accountStatementRequestDTO.StartDate && o.DateTime <= accountStatementRequestDTO.EndDate)
                                    .Select(o => new TransactionInfoDTO
                                    {
                                        AccountNumber = accountStatementRequestDTO.AccountNumber,
                                        TransactionType = o.TransactionType,
                                        Amount = o.Amount,
                                        Remark = o.Remark,
                                        TransactionDate = o.DateTime,
                                    })
                                    .OrderBy(o => o.TransactionDate)
                                    .ToList();

                responseModel.Status = true;
                responseModel.Message = "Transaction History fetched successfully";
                responseModel.Data = transactions;
            }
            catch (Exception ex)
            {
                responseModel.Status = false;
                responseModel.Message = "Error occured contact system admin";
                responseModel.Data = ex.Message;
            }

            return responseModel;
        }

        #region Private Region

        /// <summary>
        /// Function to add Transaction entries for all the Debit/Credit transactions
        /// </summary>
        /// <param name="entryDTO"></param>
        private void AddNewTransactionEntry(TransactionEntryDTO entryDTO)
        {
            try
            {
                TransactionModel model = new TransactionModel
                {
                    ReferenceNumber = entryDTO.UniqueTransactionId,
                    FromAccountId = entryDTO.FromAccountNumber,
                    ToAccountId = entryDTO.ToAccountNumber,
                    TransactionType = entryDTO.TransactionType.ToString(),
                    Remark = entryDTO.Remark,
                    Amount = entryDTO.Amount,
                    DateTime = DateTime.Now,
                    TransactionForAccountId = entryDTO.TransactionForAccountNumber,
                };

                _context.Transactions.Add(model);
                _context.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
    }
}
