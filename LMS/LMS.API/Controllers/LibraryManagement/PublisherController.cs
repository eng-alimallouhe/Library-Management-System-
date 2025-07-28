using LMS.Application.DTOs.LibraryManagement.Publishers;
using LMS.Application.Features.LibraryManagement.Publishers.Queries.GetPublisherLookup;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [ApiVersion("1.0")]
    [Route("api/{version:apiVersion}/stock/[controller]")]
    [ApiController]
    public class PublisherController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PublisherController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpGet("look-up")]
        public async Task<ActionResult<ICollection<PublisherLookupDto>>> GetPublisherLookupAsync([FromQuery] Language language)
        {
            return Ok(await _mediator.Send(new GetPublisherLookupQuery(language)));
        }
    }
}
