using AutoMapper;
using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models.Categories;
using LMS.Domain.LibraryManagement.Models.Relations;

namespace LMS.Application.MappingProfiles.LibraryManagement
{
    public class CategoryMappingProfile : Profile
    {
        public CategoryMappingProfile()
        {
            CreateMap<ProductCategory, CategoryLookUpDto>()
                .ForMember(dest => dest.CategoryName, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (context.TryGetItems(out var items) && items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                        {
                            var langEnum = (Language)langInt;
                            return src.Category.Translations?.FirstOrDefault(t => t.Language == langEnum)?.CategoryName ?? "N/A";
                        }
                        return "N/A";
                    });
                })
                .ForMember(dest => dest.CategoryId, dest => dest.MapFrom(src => src.CategoryId));
        }
    }
}
