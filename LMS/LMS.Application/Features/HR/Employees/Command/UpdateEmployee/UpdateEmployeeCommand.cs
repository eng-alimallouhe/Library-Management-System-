using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Command.UpdateEmployee
{
    public class UpdateEmployeeCommand : IRequest<Result>
    {
        public Guid EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public decimal BaseSalary { get; set; }
        public string PhoneNumber { get; set; }
        public byte[]? FaceImage { get; set; } // <--- تمت إضافة هذه الخاصية لدعم تحديث الصورة
    }
}