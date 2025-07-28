using AutoMapper;
using LMS.API.DTOs.HR.Employee;
using LMS.Application.Features.HR.Employees.Command.CreateEmployee;
using LMS.Application.Features.HR.Employees.Command.TransferEmployee;
using LMS.Application.Features.HR.Employees.Command.UpdateEmployee;

namespace LMS.API.MappingProfiles.HR
{
    public class EmployeeMappingProfile : Profile
    {
        public EmployeeMappingProfile()
        {
            CreateMap<EmployeeCreateRequestDto, CreateEmployeeCommand>()
                 .ForMember(dest => dest.AppointmentDecision, opt => opt.Ignore())
                 .ForMember(dest => dest.FaceImage, opt => opt.Ignore());

            CreateMap<EmployeeUpdateRequestDto, UpdateEmployeeCommand>()
                .ForMember(dest => dest.EmployeeId, opt => opt.MapFrom(src => Guid.NewGuid()));


            CreateMap<TransferEmployeeRequestDto, TransferEmployeeCommand>();
        }
    }
}
