using LMS.Application.DTOs.Authentication;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.Accounts.Commands.TowFactorAuthentication
{
    public record TowFactorAuthenticationCommand(string Email) : IRequest<Result<AuthorizationDto>>;
}
