using AutoMapper;
using LMS.API.DTOs.LibraryManagement.Books;
using LMS.Application.Features.LibraryManagement.Books.Commands.AddBook;
using LMS.Application.Features.LibraryManagement.Books.Commands.UpdateBook;

namespace LMS.API.MappingProfiles.LibraryManagement
{
    public class BookMappingProfile : Profile
    {
        public BookMappingProfile() 
        {
            CreateMap<BookCreateRequestDto, AddBookCommand>()
                .ForMember(dest => dest.ImageByte, opt => opt.Ignore());

            CreateMap<BookUpdateRequestDto, UpdateBookCommand>()
                .ForMember(dest => dest.ImageByte, opt => opt.Ignore())
                .ForMember(dest => dest.BookId, opt => opt.Ignore());
        }
    }
}