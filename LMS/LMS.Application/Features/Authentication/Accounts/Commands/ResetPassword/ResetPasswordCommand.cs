using LMS.Application.DTOs.Authentication;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.Accounts.Commands.ResetPassword
{
    public record ResetPasswordCommand(
        string Email,
        string NewPassword) : IRequest<Result<AuthorizationDto>>;
}
