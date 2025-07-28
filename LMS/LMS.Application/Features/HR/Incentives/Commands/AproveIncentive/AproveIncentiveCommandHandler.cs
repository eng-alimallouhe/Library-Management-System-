using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.AproveIncentive
{
    public class AproveIncentiveCommandHandler : IRequestHandler<AproveIncentiveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Incentive> _incentiveRepo;
        private readonly IUnitOfWork _unitOfWork;

        public AproveIncentiveCommandHandler(
            ISoftDeletableRepository<Incentive> incentiveRepo,
            IUnitOfWork unitOfWork)
        {
            _incentiveRepo = incentiveRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(AproveIncentiveCommand request, CancellationToken cancellationToken)
        {
            var penalty = await _incentiveRepo.GetByIdAsync(request.IncentiveId);

            if (penalty is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
            }

            if (penalty.IsPaid)
            {
                return Result.Failure($"COMMON.{ResponseStatus.RECORD_STATUS_CONNOT_BE_MODIFIED}");
            }

            if (penalty.IsApproved)
            {
                return Result.Failure($"COMMON.{ResponseStatus.STATUS_ALREADY_SET}");
            }

            penalty.IsApproved = request.IsApproved;

            penalty.DecisionDate = DateTime.UtcNow;

            if (!request.IsApproved)
            {
                penalty.IsActive = false;
            }

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success($"COMMOM.{ResponseStatus.UPDATE_COMPLETED}");
            } catch (Exception)
            {
                return Result.Failure($"COMMOM.{ResponseStatus.UNABLE_UPDATE_NOT_FOUNDED_ELEMENT}");
            }
        }
    }
}