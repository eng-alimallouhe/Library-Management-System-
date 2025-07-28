using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR;
using LMS.Domain.HR.Models;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Queries.GetLeaveToUpdate
{
    public class GetLeaveToUpdateQueryHandler : IRequestHandler<GetLeaveToUpdateQuery, LeaveUpdateDto?>
    {
        private readonly ISoftDeletableRepository<Leave> _leaveRepo;
        private readonly IMapper _mapper;

        public GetLeaveToUpdateQueryHandler(
            ISoftDeletableRepository<Leave> leaveRepo,
            IMapper mapper)
        {
            _leaveRepo = leaveRepo;
            _mapper = mapper;
        }

        public async Task<LeaveUpdateDto?> Handle(GetLeaveToUpdateQuery request, CancellationToken cancellationToken)
        {
            var leave = await _leaveRepo.GetByIdAsync(request.LeaveId);

            if (leave == null)
            {
                return null;
            }

            return _mapper.Map<LeaveUpdateDto>(leave);
        }
    }
}
