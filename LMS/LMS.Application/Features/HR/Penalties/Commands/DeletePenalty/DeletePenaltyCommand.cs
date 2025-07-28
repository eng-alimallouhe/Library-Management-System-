using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.DeletePenalty
{
    public class DeletePenaltyCommand : IRequest<Result>
    {
        public Guid PenaltyId { get; set; }
    }
}
