using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Departments.Command.CreateDepartment
{
    public class CreateDepartmentCommand : IRequest<Result>
    {
        public string DepartmentName { get; set; }
        public string DepartmentDescription { get; set; }
    }
}