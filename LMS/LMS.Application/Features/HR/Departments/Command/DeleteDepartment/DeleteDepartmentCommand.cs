using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Command.DeleteDepartment
{
    public class DeleteDepartmentCommand : IRequest<Result>
    {
        public Guid DepartmentId {  get; set; }
    }
}
