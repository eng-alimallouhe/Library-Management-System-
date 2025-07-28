using AutoMapper;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Queries.GetIncentiveById
{
    public class GetIncentiveByIdQueryHandler : IRequestHandler<GetIncentiveByIdQuery, IncentiveDetailsDto?>
    {
        private readonly IEmployeeCompensationHelper _compHelper;
        private readonly IMapper _mapper;

        public GetIncentiveByIdQueryHandler(
            IEmployeeCompensationHelper compHelper,
            IMapper mapper)
        {
            _compHelper = compHelper;
            _mapper = mapper;
        }

        public async Task<IncentiveDetailsDto?> Handle(GetIncentiveByIdQuery request, CancellationToken cancellationToken)
        {
            var result = await _compHelper.GetIncentiveByIdAsync(request.IncentiveId);
            return _mapper.Map<IncentiveDetailsDto>(result);
        }
    }
}