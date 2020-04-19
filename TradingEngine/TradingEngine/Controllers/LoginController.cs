using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TradingEngine.Requests.Query;

namespace TradingEngine.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LoginController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        [Produces("application/json")]
        [Consumes("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
        [HttpPost]
        public async Task<IActionResult> AddWallet([FromBody] UserLoginQuery query)
        {
            // simulate log in
            UserLoginQueryResult user = await _mediator.Send(query);

            var auth = Auth.Instance;

            auth.LoggedInUserId = user.Id;

            return Ok(user);
        }
    }
}