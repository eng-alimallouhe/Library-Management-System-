using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Attendances.Commands.CheckIn
{
    public class CheckInCommand : IRequest<Result>
    {
        public byte[] FaceImage { get; set; } = new byte[0];
        public string UserName { get; set; } = string.Empty;
    }
}
