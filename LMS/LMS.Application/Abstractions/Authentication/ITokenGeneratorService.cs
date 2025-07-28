using LMS.Application.DTOs.Authentication;
using LMS.Common.Results;

namespace LMS.Application.Abstractions.Authentication
{
    public interface ITokenGeneratorService
    {
        Task<Result<string>> GenerateAccessTokenAsync(Guid userId);
        Task<Result<string>> GenerateRefreshTokenAsync(Guid userId);

        Task<Result<AuthorizationDto>> ValidateTokenAsync(string refreshToken, string accessToken);
    }
}
