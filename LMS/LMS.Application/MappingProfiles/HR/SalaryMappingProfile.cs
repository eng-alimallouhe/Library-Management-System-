using AutoMapper;
using LMS.Application.DTOs.HR;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class SalaryMappingProfile : Profile
    {
        public SalaryMappingProfile()
        {
            CreateMap<Salary, SalariesOverviewDto>();
        }
    }
}
