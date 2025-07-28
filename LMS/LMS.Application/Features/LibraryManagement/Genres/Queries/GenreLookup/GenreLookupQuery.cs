using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Genres.Queries.GenreLookup
{
    public record GenreLookupQuery(Language Language) : IRequest<ICollection<GenreLookupDto>>;
}
