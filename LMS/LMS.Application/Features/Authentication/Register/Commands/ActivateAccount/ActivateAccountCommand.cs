using LMS.Application.DTOs.Authentication;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.Register.Commands.ActivateAccount
{
    public record ActivateAccountCommand(string Email) : IRequest<Result<AuthorizationDto>>;
}