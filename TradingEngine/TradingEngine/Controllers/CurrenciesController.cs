using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Requests.Query;

namespace TradingEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CurrenciesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CurrenciesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<CurrenciesQueryResult>))]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            IEnumerable<CurrenciesQueryResult> result = await _mediator.Send(new CurrenciesQuery());

            return Ok(result);
        }
    }
}