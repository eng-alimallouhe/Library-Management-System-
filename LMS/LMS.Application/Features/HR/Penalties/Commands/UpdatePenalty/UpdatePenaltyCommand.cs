using System.ComponentModel.DataAnnotations;
using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Penalties.Commands.UpdatePenalty
{
    public class UpdatePenaltyCommand : IRequest<Result>
    {
        public Guid PenaltyId { get; set; } = Guid.Empty;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Reason { get; set; } = string.Empty;


        public byte[]? DesicionFile { get; set; }
    }
}