using AutoMapper;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.UpdateIncentive
{
    public class UpdateIncentiveCommandHandler : IRequestHandler<UpdateIncentiveCommand, Result>
    {
        private readonly ISoftDeletableRepository<Incentive> _incentiveRepo;
        private readonly IAppLogger<UpdateIncentiveCommandHandler> _logger;
        private readonly IMapper _mapper;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateIncentiveCommandHandler(
            ISoftDeletableRepository<Incentive> incentiveRepo,
            IAppLogger<UpdateIncentiveCommandHandler> logger,
            IFileHostingUploaderHelper fileHostingUploaderHelper,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _incentiveRepo = incentiveRepo;
            _logger = logger;
            _fileHostingUploaderHelper = fileHostingUploaderHelper;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(UpdateIncentiveCommand request, CancellationToken cancellationToken)
        {
            var incentive = await _incentiveRepo.GetByIdAsync(request.IncentiveId!.Value);

            if (incentive is null)
                return Result.Failure($"COMMON.{ResponseStatus.UNABLE_UPDATE_NOT_FOUNDED_ELEMENT}");

            Result<String> response = Result<string>.Failure("");

            if (request.DesicionFile is not null)
            {
                response = await _fileHostingUploaderHelper.UploadPdfAsync(request.DesicionFile, $"{Guid.NewGuid().ToString()}.pdf");

                if (response.IsFailed)
                {
                    return Result.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");
                }

                incentive.DecisionFileUrl = response.Value!;
            }

            _mapper.Map(request, incentive);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError("Error while updating incentive", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }

            return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
        }
    }
}
