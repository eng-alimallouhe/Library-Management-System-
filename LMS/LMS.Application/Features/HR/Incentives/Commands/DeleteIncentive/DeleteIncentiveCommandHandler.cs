using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.DeleteIncentive
{
    public class DeleteIncentiveCommandHandler : IRequestHandler<DeleteIncentiveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Incentive> _incentiveRepo;
        private readonly IAppLogger<DeleteIncentiveCommandHandler> _logger;

        public DeleteIncentiveCommandHandler(
            ISoftDeletableRepository<Incentive> incentiveRepo,
            IAppLogger<DeleteIncentiveCommandHandler> logger)
        {
            _incentiveRepo = incentiveRepo;
            _logger = logger;
        }

        public async Task<Result> Handle(DeleteIncentiveCommand request, CancellationToken cancellationToken)
        {
            try
            {
                await _incentiveRepo.SoftDeleteAsync(request.IncentiveId);
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while deleting incentive", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            return Result.Success($"COMMON.{ResponseStatus.DELETE_COMPLETED}");
        }
    }
}
