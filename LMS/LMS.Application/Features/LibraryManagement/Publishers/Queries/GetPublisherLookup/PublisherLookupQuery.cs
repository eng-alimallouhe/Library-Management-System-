using LMS.Application.DTOs.LibraryManagement.Publishers;
using LMS.Domain.Identity.Enums;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Publishers.Queries.GetPublisherLookup
{
    public record GetPublisherLookupQuery(Language Language) : IRequest<ICollection<PublisherLookupDto>>;
}
