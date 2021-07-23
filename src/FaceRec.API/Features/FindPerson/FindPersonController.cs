using FaceRec.API.Features.AddPerson;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FaceRec.API.Features.FindPerson
{
    [ApiController]
    [Route("person")]
    public class FindPersonController : ControllerBase
    {
        private readonly ILogger<CreatePersonController> _logger;
        private readonly IMediator _mediator;

        public FindPersonController(IMediator mediator, ILogger<CreatePersonController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> FindPerson([FromBody] FindPersonCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}