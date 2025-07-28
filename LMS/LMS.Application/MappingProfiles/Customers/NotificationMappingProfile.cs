using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.Identity;
using LMS.Domain.Identity.Enums;
using LMS.Domain.Identity.Models.Notifications;

namespace LMS.Application.MappingProfiles.Customers
{
    public class NotificationMappingProfile : Profile
    {
        public NotificationMappingProfile()
        {
            CreateMap<Notification, NotificationDto>()
            .ForMember(dest => dest.CreatedAt, opt =>
                opt.MapFrom(src => src.CreatedAt.ConvertToSyrianTime()))
            .ForMember(dest => dest.Title, opt =>
                opt.MapFrom((src, dest, destMember, context) =>
                {
                    var langId = context.Items.ContainsKey("lang")
                        ? (int)context.Items["lang"]
                        : 1;

                    var translation = src.Translations
                        .FirstOrDefault(t => t.Language == (Language) langId);

                    return translation?.Title ?? string.Empty;
                }))
            .ForMember(dest => dest.Message, opt =>
                opt.MapFrom((src, dest, destMember, context) =>
                {
                    var langId = context.Items.ContainsKey("lang")
                        ? (int)context.Items["lang"]
                        : 1;

                    var translation = src.Translations
                        .FirstOrDefault(t => t.Language == (Language)langId);

                    return translation?.Message ?? string.Empty;
                }));

        }
    }
}
