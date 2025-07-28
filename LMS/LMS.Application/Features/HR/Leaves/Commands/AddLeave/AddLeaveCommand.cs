using LMS.Common.Results;
using LMS.Domain.HR.Enums;
using MediatR;

namespace LMS.Application.Features.HR.Leaves.Commands.AddLeave
{
    public class AddLeaveCommand : IRequest<Result>
    {
        public Guid EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int LeaveType { get; set; }
        public string Reason {  get; set; }
    }
}