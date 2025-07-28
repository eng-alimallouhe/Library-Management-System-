using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.AproveIncentive
{
    public class AproveIncentiveCommand : IRequest<Result>
    {
        public Guid IncentiveId { get; set; }
        public bool IsApproved { get; set; }
    }
}
