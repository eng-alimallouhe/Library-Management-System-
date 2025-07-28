using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.AddPenalty
{
    public class AddPenaltyCommand : IRequest<Result>
    {
        public Guid EmployeeId { get; set; } = Guid.Empty;
        public decimal Amount { get; set; }
        public string Reason {  get; set; }
        public byte[] DecisionFile { get; set; } = new byte[0];
    }
}