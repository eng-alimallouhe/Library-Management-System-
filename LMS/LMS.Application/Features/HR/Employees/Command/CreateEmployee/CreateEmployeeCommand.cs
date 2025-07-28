using System.ComponentModel.DataAnnotations;
using LMS.Application.DTOs.HR.Employees;
using LMS.Common.Results;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.HR.Employees.Command.CreateEmployee
{
    public class CreateEmployeeCommand : IRequest<Result<EmployeeCreatedDto>>
    {
        [Required]
        public string FullName { get; init; }

        [Required, EmailAddress]
        public string Email { get; init; }

        [Required]
        public string PhoneNumber { get; init; }

        [Required]
        public Language Language { get; init; }

        [Required]
        public Guid DepartmentId { get; init; }

        
        [Required, Range(0, 999999)]
        public decimal BaseSalary { get; init; }


        public byte[] AppointmentDecision { get; set; }
        
        public byte[] FaceImage { get; set; }
    }

}
