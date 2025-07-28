using AutoMapper;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Queries.GetLeaveById
{
    public class GetLeaveByIdQueryHandler : IRequestHandler<GetLeaveByIdQuery, LeaveDetailsDto?>
    {
        private readonly IEmployeeCompensationHelper _employeeHelper;
        private readonly IMapper _mapper;

        public GetLeaveByIdQueryHandler(
            IEmployeeCompensationHelper employeeHelper, 
            IMapper mapper)
        {
            _employeeHelper = employeeHelper;
            _mapper = mapper;
        }

        public async Task<LeaveDetailsDto?> Handle(GetLeaveByIdQuery request, CancellationToken cancellationToken)
        {
            var leave = await _employeeHelper.GetLeaveByIdAsync(request.LeaveId);

            if (leave == null)
            {
                return null;
            }

            var leaveDto = _mapper.Map<LeaveDetailsDto>(leave);

            return leaveDto;
        }
    }
}