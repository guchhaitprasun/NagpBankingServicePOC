using DocumentService.MessageBroker;
using Grpc.Net.Client;
using Newtonsoft.Json;
using SharedProject.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.gRPC.Client
{
    public class AccountRpcClientService
    {
        public static string _gRPCServierURL;

        public AccountRpcClientService()
        {
            _gRPCServierURL = "https://localhost:44336";
        }

        public bool FetchAndGenerateAccountStatementPDF(string payload)
        {
            try
            {
                using (var channel = GetGrpcChannel())
                {
                    var client = new AccountStatement.AccountStatementClient(channel);

                    // Create a Raw JSON request
                    var Json = new JsonRequest
                    {
                        Json = payload
                    };

                    Console.WriteLine("\n:::: Fetching Account Statement from Accounts Service using gRPC ::::");
                    var resp = client.FetchAccountStatement(Json);
                    GeneratePDF(resp);

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        private GrpcChannel GetGrpcChannel()
        {
            return GrpcChannel.ForAddress(_gRPCServierURL);
        }

        private void GeneratePDF(JsonResponse resp)
        {
            int accountNo = int.MinValue;
            var responses = JsonConvert.DeserializeObject<ResponseModel>(resp.Statement);
            var data = JsonConvert.DeserializeObject<List<TransactionInfoDTO>>(JsonConvert.SerializeObject(responses.Data));

            if (data.Count == 0)
            {
                Console.WriteLine("Transaction History Not Available");
            }

            foreach (var transactions in data)
            {
                Console.WriteLine($"Account Number: {transactions.AccountNumber}");
                Console.WriteLine($"Transaction Type: {transactions.TransactionType}");
                Console.WriteLine($"Amount: {transactions.Amount}");
                Console.WriteLine($"Remark: {transactions.Remark}");
                Console.WriteLine($"Transaction Date: {transactions.TransactionDate} \n\n");

                accountNo = transactions.AccountNumber;

            }
            Console.WriteLine(":::: PDF Generation End, Sending Notification to Notification Service 1 & 2 ::::");
        }
    }
}
