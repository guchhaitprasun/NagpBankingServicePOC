using AccountService.Business;
using AccountService.MessageBroker;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SharedProject.DTOs;

namespace AccountService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IBusinessLayer _businessLayer;
        private readonly IQueuePublisher<string> _queuePublisher;


        public AccountController(IBusinessLayer businessLayer, IQueuePublisher<string> queuePublisher)
        {
            _businessLayer = businessLayer;
            _queuePublisher = queuePublisher;
        }


        [HttpPost]
        [Route("create")]
        public IActionResult CreateAccount([FromBody] AccountRegistrationDTO accountRegistrationDTO)
        {
            var resp = _businessLayer.CreateNewAccount(accountRegistrationDTO);
            if (resp.Status)
            {
                _queuePublisher.PublishMessageAsync("event_account_created");
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
            return Ok();
        }



    }
}
