using LMS.Application.DTOs.LibraryManagement.Authors;
using LMS.Application.Features.LibraryManagement.Authors.Queries.GetAuthorLookup;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{

    [Route("api/v{version:apiVersion}/stock/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthorController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<ICollection<AuthorLookupDto>>> GetAuthorLookupAsync([FromQuery] Language language)
        {
            return Ok(await _mediator.Send(new GetAuthorLookupQuery(language)));
        }
    }
}
