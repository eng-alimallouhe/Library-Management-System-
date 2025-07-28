using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Authors.Queries.GetAuthorLookup
{
    public record GetAuthorLookupQuery(Language Language) : IRequest<ICollection<AuthorLookupDto>>;
}
