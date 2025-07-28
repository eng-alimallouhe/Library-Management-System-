using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.HR;
using LMS.Application.Features.HR.Holidays.Commands.CreateHoliday;
using LMS.Application.Features.HR.Holidays.Commands.UpdateHoliday;
using LMS.Domain.HR.Models;

namespace LMS.Application.MappingProfiles.HR
{
    public class HolidayMappingProfile : Profile
    {
        public HolidayMappingProfile()
        {
            CreateMap<CreateHolidayCommand, Holiday>();
            
            CreateMap<UpdateHolidayCommand, Holiday>()
                .ForMember(dest => dest.HolidayId, opt => opt.Ignore());

            CreateMap<Holiday, HolidayToUpdateDto>();
            
            CreateMap<Holiday, HolidayDetailsDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertToSyrianTime()))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertToSyrianTime()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ConvertToSyrianTime()))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt.ConvertToSyrianTime()));



            CreateMap<Holiday, HolidayOverviewDto>()
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate.ConvertToSyrianTime()))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate.ConvertToSyrianTime()));
        }
    }
}
