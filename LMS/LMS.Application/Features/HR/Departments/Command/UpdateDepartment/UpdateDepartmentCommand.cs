using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Command.UpdateDepartment
{
    public class UpdateDepartmentCommand : IRequest<Result>
    {
        public Guid DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
    }
}