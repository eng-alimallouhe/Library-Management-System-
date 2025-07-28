using AutoMapper;
using LMS.API.DTOs.HR.Department;
using LMS.Application.Features.HR.Departments.Command.CreateDepartment;
using LMS.Application.Features.HR.Departments.Command.UpdateDepartment;

namespace LMS.API.MappingProfiles.HR
{
    public class DepartmentMappingProfile : Profile
    {
        public DepartmentMappingProfile()
        {
            CreateMap<DepartmentRequestDto, CreateDepartmentCommand>();
            
            CreateMap<DepartmentRequestDto, UpdateDepartmentCommand>()
                .ForMember(dest => dest.DepartmentId, 
                opt => opt.MapFrom(src => Guid.Empty));
        }
    }
}
