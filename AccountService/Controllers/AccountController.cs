using AccountService.Business;
using AccountService.MessageBroker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SharedProject.DTOs;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IBusinessLayer _businessLayer;
        private readonly IQueuePublisher<string> _queuePublisher;
        private readonly IQueuePublisher<AccountStatementRequestDTO> _queuePublisher2;


        public AccountController(IBusinessLayer businessLayer, IQueuePublisher<string> queuePublisher, IQueuePublisher<AccountStatementRequestDTO> queuePublisher2)
        {
            _businessLayer = businessLayer;
            _queuePublisher = queuePublisher;
            _queuePublisher2 = queuePublisher2;
        }


        [HttpPost]
        [Route("create")]
        public IActionResult CreateAccount([FromBody] AccountRegistrationDTO accountRegistrationDTO)
        {
            var resp = _businessLayer.CreateNewAccount(accountRegistrationDTO);
            if (resp.Status)
            {
                PublishMessage("event_account_created", resp.Data);
                return Ok(resp);
            }

            return BadRequest(resp);
        }

        [HttpPost]
        [Route("addfunds")]
        public IActionResult AddMoney([FromBody] AddfundsDTO addfundsDTO)
        {
            var resp = _businessLayer.Addfunds(addfundsDTO);
            if (resp.Status)
            {
                return Ok(resp);
            }

            return BadRequest(resp);
        }

        [HttpPost]
        [Route("sendmoney")]
        public IActionResult SendMoney([FromBody] SendMoneyDTO sendMoneyDTO)
        {
            var resp = _businessLayer.Sendmoney(sendMoneyDTO);
            if (resp.Status)
            {
                return Ok(resp);
            }

            return BadRequest(resp);
        }

        [HttpPost]
        [Route("statement")]
        public IActionResult GetAccountStatement([FromBody] AccountStatementRequestDTO accountStatementRequestDTO)
        {
            var resp = _businessLayer.AccountStatement(accountStatementRequestDTO);
            if (resp.Status)
            {
                return Ok(resp);
            }

            return BadRequest(resp);
        }

        [HttpPost]
        [Route("statement/pdf")]
        public IActionResult GetPdfStatement([FromBody] AccountStatementRequestDTO accountStatementRequestDTO)
        {
            _queuePublisher2.PublishAccountStatementPdfRequestAsync(accountStatementRequestDTO);
            return Ok("PDF statement will be send to registered email address");
        }


        #region Private region 

        private void PublishMessage(string eventType, object data)
        {
            var messageToBroker = new
            {
                Event = eventType,
                AccountDetails = data
            };

            _queuePublisher.PublishMessageAsync(JsonConvert.SerializeObject(messageToBroker));
        }
        #endregion


    }
}
