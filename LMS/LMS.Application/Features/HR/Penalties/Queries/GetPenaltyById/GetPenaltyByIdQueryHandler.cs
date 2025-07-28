using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Queries.GetPenaltyById
{
    public class GetPenaltyByIdQueryHandler : IRequestHandler<GetPenaltyByIdQuery, PenaltyDetailsDto?>
    {
        private readonly IEmployeeCompensationHelper _employeeCompensationHelper;
        private readonly IMapper _mapper;

        public GetPenaltyByIdQueryHandler(
            IEmployeeCompensationHelper employeeCompensationHelper,
            IMapper mapper)
        {
            _employeeCompensationHelper = employeeCompensationHelper;
            _mapper = mapper;
        }

        public async Task<PenaltyDetailsDto?> Handle(GetPenaltyByIdQuery request, CancellationToken cancellationToken)
        {
            var reponse = await _employeeCompensationHelper.GetPenaltyByIdAsync(request.PenaltyId);
            return _mapper.Map<PenaltyDetailsDto>(reponse);
        }
    }
}
