using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Categories.Queries.GetCategoriesLookup
{
    public record GetCategoriesLookupQuery(Language Language) : IRequest<ICollection<CategoryLookUpDto>>;
}
