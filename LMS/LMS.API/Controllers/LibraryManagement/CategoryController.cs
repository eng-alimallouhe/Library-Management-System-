using LMS.Application.DTOs.LibraryManagement.Categories;
using LMS.Application.Features.LibraryManagement.Categories.Queries.GetCategoriesLookup;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/stock/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        
        public CategoryController(IMediator mediator) 
        {
            _mediator = mediator;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<ICollection<CategoryLookUpDto>>> GetCategoryLookupAsync(Language language)
        {
            return Ok(await _mediator.Send(new GetCategoriesLookupQuery(language)));
        }
    }
}
