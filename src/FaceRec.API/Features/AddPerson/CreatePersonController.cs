using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace FaceRec.API.Features.AddPerson
{
    [ApiController]
    [Route("person")]
    public class CreatePersonController : ControllerBase
    {
        private readonly ILogger<CreatePersonController> _logger;
        private readonly IMediator _mediator;

        public CreatePersonController(IMediator mediator, ILogger<CreatePersonController> logger)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> AddPerson([FromBody] CreatePersonCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _mediator.Send(command);
            return result != -1 ? Ok(new { Id = result }) : Conflict(new { Error = "Unable to create person" });
        }
    }
}