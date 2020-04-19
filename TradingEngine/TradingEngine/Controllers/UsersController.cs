using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Requests.Commands;
using TradingEngine.Requests.Query;

namespace TradingEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UsersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<UsersQueryResult>))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<UsersQueryResult> result = await _mediator.Send(new UsersQuery());

            return Ok(result);
        }

        // For testing
        //[Route("add-wallet")]
        //[Produces("application/json")]
        //[Consumes("application/json")]
        //[ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        //[HttpPost]
        //public async Task<IActionResult> AddWallet([FromBody] AddWalletCommand command)
        //{
        //    bool result = await _mediator.Send(new AddWalletCommand(command.InitialBalance));

        //    return Ok(result);
        //}

        [Route("check-balance")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CheckBalanceQueryResult))]
        [HttpGet]
        public async Task<IActionResult> CheckBalance()
        {
            CheckBalanceQueryResult result = await _mediator.Send(new CheckBalanceQuery());

            return Ok(result);
        }

        [Route("deposit")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> Deposit([FromBody] DepositCommand command)
        {
            bool result = await _mediator.Send(command);

            return Ok(result);
        }

        [Route("withdraw")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> Withdraw([FromBody] WithdrawCommand command)
        {
            bool result = await _mediator.Send(command);

            return Ok(result);
        }

        [Route("transfer-funds")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> TransferFunds([FromBody] TransferFundsCommand command)
        {
            bool result = await _mediator.Send(command);

            return Ok(result);
        }

        [Route("transactions/history")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<TransactionHistoryQueryResult>))]
        [HttpGet]
        public async Task<IActionResult> TransactionHistory()
        {
            IEnumerable<TransactionHistoryQueryResult> result = await _mediator.Send(new TransactionHistoryQuery());

            return Ok(result);
        }
    }
}