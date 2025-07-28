using AutoMapper;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.UpdatePenalty
{
    public class UpdatePenaltyCommandHandler : IRequestHandler<UpdatePenaltyCommand, Result>
    {
        private readonly ISoftDeletableRepository<Penalty> _penaltyRepo;
        private readonly IAppLogger<UpdatePenaltyCommandHandler> _appLogger;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdatePenaltyCommandHandler(
            ISoftDeletableRepository<Penalty> penaltyRepo,
            IAppLogger<UpdatePenaltyCommandHandler> appLogger,
            IFileHostingUploaderHelper fileHostingUploaderHelper,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _penaltyRepo = penaltyRepo;
            _appLogger = appLogger;
            _fileHostingUploaderHelper = fileHostingUploaderHelper;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdatePenaltyCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var penalty = await _penaltyRepo.GetByIdAsync(request.PenaltyId);


                if (penalty is null)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.UNABLE_UPDATE_NOT_FOUNDED_ELEMENT}");
                }

                Result<String> response = Result<string>.Failure("");
            
                if (request.DesicionFile is not null)
                {
                    response = await _fileHostingUploaderHelper.UploadPdfAsync(request.DesicionFile, $"{Guid.NewGuid().ToString()}.pdf");

                    if (response.IsFailed)
                    {
                        return Result.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");
                    }
                    penalty.DecisionFileUrl = response.Value!;
                }


                _mapper.Map(request, penalty);
            
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _appLogger.LogError("Error while updating an penalty", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
        }
    }
}
