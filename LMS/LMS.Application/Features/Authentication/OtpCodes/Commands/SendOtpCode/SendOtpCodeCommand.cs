﻿using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.OtpCodes.Commands.SendOtpCode
{
    public record SendOtpCodeCommand(string Email,  int CodeType) : IRequest<Result>;
}