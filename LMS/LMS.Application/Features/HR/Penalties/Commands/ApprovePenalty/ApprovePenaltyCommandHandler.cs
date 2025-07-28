using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.ApprovePenalty
{
    public class ApprovePenaltyCommandHandler : IRequestHandler<ApprovePenaltyCommand, Result>
    {
        private readonly ISoftDeletableRepository<Penalty> _penaltyRepo;
        private readonly IUnitOfWork _unitOfWork;

        public ApprovePenaltyCommandHandler(
            ISoftDeletableRepository<Penalty> penaltyRepo,
            IUnitOfWork unitOfWork)
        {
            _penaltyRepo = penaltyRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ApprovePenaltyCommand request, CancellationToken cancellationToken)
        {
            var penalty = await _penaltyRepo.GetByIdAsync(request.PenaltyId);

            if (penalty is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
            }

            if (penalty.IsApproved)
            {
                return Result.Failure($"COMMON.{ResponseStatus.STATUS_ALREADY_SET}");
            }

            if (request.IsAproved)
            {
                penalty.IsApproved = true;
                penalty.DecisionDate = DateTime.UtcNow;
            }
            else
            {
                penalty.IsActive = false;
                penalty.IsApproved = false;
            }


            try
            {
                await _unitOfWork.SaveChangesAsync();
                return Result.Success($"COMMOM.{ResponseStatus.UPDATE_COMPLETED}");
            } catch (Exception)
            {
                return Result.Failure($"COMMOM.{ResponseStatus.UNABLE_UPDATE_NOT_FOUNDED_ELEMENT}");
            }
        }
    }
}
