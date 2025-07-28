using System.ComponentModel.DataAnnotations;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Incentives.Commands.UpdateIncentive
{
    public record UpdateIncentiveCommand : IRequest<Result>
    {
        public Guid? IncentiveId { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;

        public byte[]? DesicionFile { get; set; }
    }
}