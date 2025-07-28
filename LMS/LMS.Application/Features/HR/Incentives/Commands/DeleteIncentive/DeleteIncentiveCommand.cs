using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.DeleteIncentive
{
    public class DeleteIncentiveCommand : IRequest<Result>
    {
        public Guid IncentiveId { get; set; }
    }
}