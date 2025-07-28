using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.OtpCodes.Commands.VerifyOtpCode
{
    public record VerifyOtpCodeCommand(string Email, string Code, int CodeType) : IRequest<Result>;
}