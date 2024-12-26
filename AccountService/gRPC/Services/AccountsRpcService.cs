using AccountService.Business;
using AccountService.gRPC.Proto;
using Grpc.Core;
using Newtonsoft.Json;
using SharedProject.DTOs;

namespace AccountService.gRPC.Services
{
    public class AccountsRpcService : AccountStatement.AccountStatementBase
    {
        private readonly IBusinessLayer _businessLogic;
        public AccountsRpcService(IBusinessLayer buisness)
        {
            _businessLogic = buisness;
        }

        public override Task<JsonResponse> FetchAccountStatement(JsonRequest request, ServerCallContext context)
        {
            var _request = JsonConvert.DeserializeObject<AccountStatementRequestDTO>(request.Json);
            var statement = _businessLogic.AccountStatement(_request);

            return Task.FromResult(new JsonResponse
            {
                Statement = JsonConvert.SerializeObject(statement)
            });
        }
    }
}
