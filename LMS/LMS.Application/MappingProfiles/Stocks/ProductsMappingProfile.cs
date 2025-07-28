using AutoMapper;
using LMS.Application.DTOs.Admin.Dashboard;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.DTOs.Stock;
using LMS.Domain.Identity.Enums;
using LMS.Domain.LibraryManagement.Models;
using LMS.Domain.LibraryManagement.Models.Products;

namespace LMS.Application.MappingProfiles.Stocks
{
    public class ProductsMappingProfile : Profile
    {
        public ProductsMappingProfile()
        {
            CreateMap<InventoryLog, InventoryLogOverviewDto>()
                .ForMember(dest => dest.ProductName, 
                opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (context.Items.TryGetValue("lang", out var langObj) &&langObj is int langInt)
                    {
                        var langEnum = (Language)langInt;
                        return src.Product.Translations.FirstOrDefault(t => t.Language == langEnum)?.ProductName ?? "N/A";
                    }
                    return "N/A";
                }));


            CreateMap<Product, StockSnapshotDto>()
            .ForMember(dest => dest.ProductName, opt =>
            {
                opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (context.TryGetItems(out var items) && items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                    {
                        var langEnum = (Language)langInt;
                        return src.Translations?.FirstOrDefault(t => t.Language == langEnum)?.ProductName ?? "N/A";
                    }
                    return "N/A";
                });
            })
            .ForMember(dest => dest.LogsCount, opt =>
            {
                opt.MapFrom(src => src.Logs.Count);
            });
            
            CreateMap<Product, DeadStockDto>()
                .ForMember(dest => dest.ProductName,
                opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    if (context.Items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                    {
                        var langEnum = (Language)langInt;
                        return src.Translations.FirstOrDefault(t => t.Language == langEnum)?.ProductName ?? "N/A";
                    }
                    return "N/A";
                }));


            CreateMap<Product, ProductDetailsDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.ProductName, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (context.TryGetItems(out var items) && items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                        {
                            var langEnum = (Language)langInt;
                            return src.Translations?.FirstOrDefault(t => t.Language == langEnum)?.ProductName ?? "N/A";
                        }
                        return "N/A";
                    });
                })
                .ForMember(dest => dest.ProductDescription, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (context.TryGetItems(out var items) && items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                        {
                            var langEnum = (Language)langInt;
                            return src.Translations?.FirstOrDefault(t => t.Language == langEnum)?.ProductDescription ?? "N/A";
                        }
                        return "N/A";
                    });
                })
                .ForMember(dest => dest.DiscountPercentage, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        var discount = src.Discounts.FirstOrDefault(d => d.IsActive);
                        if (discount != null && discount.EndDate > DateTime.UtcNow && discount.StartDate > DateTime.UtcNow)
                        {
                            return discount.DiscountPercentage;
                        }
                        return 0;
                    });
                })
                .ForMember(dest => dest.ProductStock, opt => opt.MapFrom(src => src.ProductStock))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.ImgUrl, opt => opt.MapFrom(src => src.ImgUrl))
                .ForMember(dest => dest.Logs, opt => opt.MapFrom(src => src.Logs))
                .ForMember(dest => dest.Categories, opt => opt.Ignore());

            CreateMap<Product, ProductOverviewDto>()
                .ForMember(dest => dest.ProductName, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (context.TryGetItems(out var items) && items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                        {
                            var langEnum = (Language)langInt;
                            return src.Translations?.FirstOrDefault(t => t.Language == langEnum)?.ProductName ?? "N/A";
                        }
                        return "N/A";
                    });
                })
                .ForMember(dest => dest.ProductDescription, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        if (context.TryGetItems(out var items) && items.TryGetValue("lang", out var langObj) && langObj is int langInt)
                        {
                            var langEnum = (Language)langInt;
                            return src.Translations?.FirstOrDefault(t => t.Language == langEnum)?.ProductDescription ?? "N/A";
                        }
                        return "N/A";
                    });
                })
                .ForMember(dest => dest.DiscountPercentage, opt =>
                {
                    opt.MapFrom((src, dest, destMember, context) =>
                    {
                        var discount = src.Discounts.FirstOrDefault(d => d.IsActive);
                        if (discount != null && discount.EndDate > DateTime.UtcNow && discount.StartDate > DateTime.UtcNow)
                        {
                            return discount.DiscountPercentage;                           
                        }
                        return 0;
                    });
                });
        }
    }
}
