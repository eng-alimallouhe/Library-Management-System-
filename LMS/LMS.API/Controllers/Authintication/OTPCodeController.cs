﻿using AutoMapper;
using LMS.API.DTOs.Authentication;
using LMS.Application.Features.Authentication.OtpCodes.Commands.SendOtpCode;
using LMS.Application.Features.Authentication.OtpCodes.Commands.VerifyOtpCode;
using LMS.Common.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.Authintication
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/authentication/[controller]")]
    [ApiController]
    public class OTPCodeController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public OTPCodeController(
            IMapper mapper,
            IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]    
        [HttpPost("send-otpcode")]
        public async Task<ActionResult<Result>> SendOtpCode(OtpCodeSendRequstDTO requst)
        {
            var command = _mapper.Map<SendOtpCodeCommand>(requst);

            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return BadRequest(response);
            }
            return Ok(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPost("verify")]
        public async Task<ActionResult<Result>> Verify(OtpVerifyRequest request)
        {
            var command = _mapper.Map<VerifyOtpCodeCommand>(request);

            var response = await _mediator.Send(command);

            if (response.IsFailed)
            {
                return NotFound(response);
            }
            return Ok(response);
        }
    }
}
