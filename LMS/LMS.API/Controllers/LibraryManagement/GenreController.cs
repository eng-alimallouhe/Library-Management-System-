using LMS.Application.DTOs.LibraryManagement.Genres;
using LMS.Application.Features.LibraryManagement.Genres.Queries.GenreLookup;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [Route("api/{version:apiVersion}/stock/[controller]")]
    [ApiController]
    public class GenreController : ControllerBase
    {
        private readonly IMediator _mediator;

        public GenreController(IMediator mediator)
        {
            _mediator = mediator;   
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<ICollection<GenreLookupDto>>> GetGenreLookupAsync([FromQuery] Language language)
        {
            return Ok(await _mediator.Send(new GenreLookupQuery(language)));
        }
    }
}
