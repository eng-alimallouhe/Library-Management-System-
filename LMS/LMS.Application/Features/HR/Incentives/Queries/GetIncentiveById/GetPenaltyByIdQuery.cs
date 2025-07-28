using System.ComponentModel.DataAnnotations;
using LMS.Application.DTOs.HR;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Queries.GetIncentiveById
{
    public record GetIncentiveByIdQuery(Guid IncentiveId) : IRequest<IncentiveDetailsDto?>;
}