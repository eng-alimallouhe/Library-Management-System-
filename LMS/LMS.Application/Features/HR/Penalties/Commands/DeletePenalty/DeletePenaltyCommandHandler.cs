using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.DeletePenalty
{
    public class DeletePenaltyCommandHandler : IRequestHandler<DeletePenaltyCommand, Result>
    {
        private readonly ISoftDeletableRepository<Penalty> _penaltyRepo;
        private readonly IAppLogger<DeletePenaltyCommandHandler> _appLogger;

        public DeletePenaltyCommandHandler(
            ISoftDeletableRepository<Penalty> penaltyRepo,
            IAppLogger<DeletePenaltyCommandHandler> appLogger)
        {
            _penaltyRepo = penaltyRepo;
            _appLogger = appLogger;
        }

        public async Task<Result> Handle(DeletePenaltyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _penaltyRepo.SoftDeleteAsync(request.PenaltyId);
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Error while deleting a penalty", ex);
                Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
        }
    }
}
