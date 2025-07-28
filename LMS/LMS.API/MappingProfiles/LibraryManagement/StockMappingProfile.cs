using AutoMapper;
using LMS.API.DTOs.LibraryManagement.Products;
using LMS.Application.Features.LibraryManagement.Products.Commands.AddProduct;
using LMS.Application.Features.LibraryManagement.Products.Commands.UpdateProduct;

namespace LMS.API.MappingProfiles.LibraryManagement
{
    public class StockMappingProfile : Profile
    {
        public StockMappingProfile()
        {
            CreateMap<ProductCreateRequestDto, AddProductCommand>()
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore());

            CreateMap<ProductCreateRequestDto, UpdateProductCommand>()
                .ForMember(dest => dest.ProductId, opt => opt.Ignore())
                .ForMember(dest => dest.ImageFile, opt => opt.Ignore());
        }
    }
}
