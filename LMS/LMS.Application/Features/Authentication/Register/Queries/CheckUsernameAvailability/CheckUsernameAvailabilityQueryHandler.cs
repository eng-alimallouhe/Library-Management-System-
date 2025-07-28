using LMS.Application.Abstractions.Repositories;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Identity.Models;
using MediatR;

namespace LMS.Application.Features.Authentication.Register.Queries.CheckUsernameAvailability
{
    public class CheckUsernameAvailabilityQueryHandler : IRequestHandler<CheckUsernameAvailabilityQuery, Result>
    {
        private readonly ISoftDeletableRepository<User> _userRepo;

        public CheckUsernameAvailabilityQueryHandler(
            ISoftDeletableRepository<User> userRepo)
        {
            _userRepo = userRepo;
        }

        public async Task<Result> Handle(CheckUsernameAvailabilityQuery request, CancellationToken cancellationToken)
        {
            var user = await _userRepo.GetByExpressionAsync(user => user.UserName == request.UserName, false);

            if (user is null)
            {
                return Result.Success($"AUTHENTICATION.{ResponseStatus.USER_NAME_AVALIABLE}");
            }
            return Result.Failure($"AUTHENTICATION.{ResponseStatus.USER_NAME_UNAVALIABLE}");
        }
    }
}
