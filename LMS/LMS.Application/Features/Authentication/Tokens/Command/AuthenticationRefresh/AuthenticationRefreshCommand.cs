using LMS.Application.DTOs.Authentication;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.Tokens.Command.AuthenticationRefresh
{
    public record AuthenticationRefreshCommand(string RefreshToken, string AccessToken) : IRequest<Result<AuthorizationDto>>;
}