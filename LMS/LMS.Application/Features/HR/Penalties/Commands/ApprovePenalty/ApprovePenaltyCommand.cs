using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.ApprovePenalty
{
    public class ApprovePenaltyCommand : IRequest<Result>
    {
        public Guid PenaltyId { get; set; }
        public bool IsAproved { get; set; }
    }
}
