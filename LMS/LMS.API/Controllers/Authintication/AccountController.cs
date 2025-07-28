using AutoMapper;
using LMS.API.DTOs.Authentication;
using LMS.Application.DTOs.Authentication;
using LMS.Application.Features.Authentication.Accounts.Commands.LogIn;
using LMS.Application.Features.Authentication.Accounts.Commands.ResetPassword;
using LMS.Application.Features.Authentication.Accounts.Commands.TowFactorAuthentication;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Authintication
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/authentication/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public AccountController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpPost("log-in")]
        public async Task<ActionResult<Result<AuthorizationDto>>> Login(LoginRequestDTO request)
        {
            var command = _mapper.Map<LogInCommand>(request);

            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpGet("tow-factor-verify/{email}")]
        public async Task<ActionResult<Result<AuthorizationDto>>> CheckTowFactor(string email)
        {
            var response = await _mediator.Send(new TowFactorAuthenticationCommand(email));

            if (response.IsFailed)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("reset-password")]
        public async Task<ActionResult<Result<AuthorizationDto>>> ResetPassword(ResetPasswordDto request)
        {
            var command = _mapper.Map<ResetPasswordCommand>(request);
            
            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return Unauthorized(response);
            }

            return Ok(response);
        }
    }
}