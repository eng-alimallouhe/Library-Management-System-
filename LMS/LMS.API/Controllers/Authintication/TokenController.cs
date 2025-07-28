using AutoMapper;
using LMS.API.DTOs.Authentication;
using LMS.Application.DTOs.Authentication;
using LMS.Application.Features.Authentication.Tokens.Command.AuthenticationRefresh;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Authintication
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/authentication/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TokenController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpPost("auth-refresh")]
        public async Task<ActionResult<Result<AuthorizationDto>>> ValidateAuth(AuthorizationRequestDTO request)
        {
            var command = _mapper.Map<AuthenticationRefreshCommand>(request);

            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return Unauthorized(response);
            }
            return Ok(response);
        }
    }
}
