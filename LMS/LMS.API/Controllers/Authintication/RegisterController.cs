using AutoMapper;
using LMS.API.DTOs.Authentication;
using LMS.Application.DTOs.Authentication;
using LMS.Application.Features.Authentication.Register.Commands.ActivateAccount;
using LMS.Application.Features.Authentication.Register.Commands.CreateTempAccount;
using LMS.Application.Features.Authentication.Register.Queries.CheckUsernameAvailability;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Authintication
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/authentication/[controller]")]
    [ApiController]
    public class RegisterController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public RegisterController(
            IMediator mediator,
            IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }



        [MapToApiVersion("1.0")]
        [HttpGet("username-availability/{username}", Name = "CheckUsernameAvailability")]
        public async Task<ActionResult<Result>> CheckUsernameAvailability(string username)
        {
            var response = await _mediator.Send(new CheckUsernameAvailabilityQuery(username));

            if (response.IsFailed)
            {
                return BadRequest(response);
            }

            return Ok(response);
        }



        [MapToApiVersion("1.0")]
        [HttpPost("create-temp-account")]
        public async Task<ActionResult<Result>> CreateTempAccount([FromForm]RegisterRequestDTO request)
        {
            var command = _mapper.Map<CreateTempAccountCommand>(request);

            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return BadRequest(response);
            }

            return Created();
        }



        [MapToApiVersion("1.0")]
        [HttpGet("activate-account/{email}")]
        public async Task<ActionResult<Result<AuthorizationDto>>> ActivateAccount(string email)
        {
            var command = new ActivateAccountCommand(email);

            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}
