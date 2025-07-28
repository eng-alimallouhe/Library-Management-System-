using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.CreateIncentive
{
    public class CreateIncentiveCommand : IRequest<Result>
    {
        public Guid EmployeeId { get; set; }
        public decimal Amount { get; set; }
        public string Reason { get; set; }
        public byte[] DecisionFile { get; set; }
    }
}
