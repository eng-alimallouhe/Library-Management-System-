using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Queries.GetPenaltyById
{
    public record GetPenaltyByIdQuery(Guid PenaltyId) : IRequest<PenaltyDetailsDto?>;
}
