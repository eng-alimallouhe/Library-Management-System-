using LMS.Application.Abstractions.Authentication;
using LMS.Application.DTOs.Authentication;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.Authentication.Tokens.Command.AuthenticationRefresh
{
    public class AuthenticationRefreshCommandHandler : IRequestHandler<AuthenticationRefreshCommand, Result<AuthorizationDto>>
    {
        private readonly ITokenGeneratorService _tokenGeneratorService;

        public AuthenticationRefreshCommandHandler(
            ITokenGeneratorService tokenGeneratorService)
        {
            _tokenGeneratorService = tokenGeneratorService;
        }

        public async Task<Result<AuthorizationDto>> Handle(AuthenticationRefreshCommand request, CancellationToken cancellationToken)
        {
            return await _tokenGeneratorService.ValidateTokenAsync(request.RefreshToken, request.AccessToken);
        }
    }
}
