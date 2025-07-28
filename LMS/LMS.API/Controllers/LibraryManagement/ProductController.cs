using AutoMapper;
using DocumentFormat.OpenXml.Office2010.CustomUI;
using LMS.API.DTOs.LibraryManagement.Products;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.Features.LibraryManagement.Products.Commands.AddProduct;
using LMS.Application.Features.LibraryManagement.Products.Commands.DeleteProduct;
using LMS.Application.Features.LibraryManagement.Products.Commands.UpdateProduct;
using LMS.Application.Features.LibraryManagement.Products.Queries.GetAllProducts;
using LMS.Application.Features.LibraryManagement.Products.Queries.GetProductById;
using LMS.Application.Features.LibraryManagement.Products.Queries.GetProductToUpdate;
using LMS.Application.Filters.Inventory;
using LMS.Common.Results;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/stock/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ProductController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }



        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<ActionResult<PagedResult<ProductOverviewDto>>> GetAllProductsAsync([FromQuery] ProductFilter filter)
        {
            return Ok(await _mediator.Send(new GetAllProductsQuery(filter)));
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<PagedResult<ProductOverviewDto>>> GetProductByIdAsync(Guid id, [FromQuery]Language language)
        {
            var response = await _mediator.Send(new GetProductByIdQuery(id, (int) language));
            return response != null? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpGet("to-update/{id:guid}")]
        public async Task<ActionResult<PagedResult<ProductOverviewDto>>> GetProductToUpdateAsync(Guid id, [FromQuery] Language language)
        {
            var response = await _mediator.Send(new GetProductToUpdateQuery(id, language));
            return response != null ? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> AddProductAsync([FromForm] ProductCreateRequestDto product)
        {
            var comaand = _mapper.Map<AddProductCommand>(product);

            using var stream = product.ImageFile.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileByByte = memoryStream.ToArray();

            comaand.ImageFile = fileByByte;

            var response = await _mediator.Send(comaand);

            return response != null? Ok(response) : BadRequest(response);
        }


        [MapToApiVersion("1.0")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> UpdateProductAsunc(Guid id, [FromForm] ProductUpdateRequestDto product)
        {
            var command = _mapper.Map<UpdateProductCommand>(product);

            if (product.ImageFile != null)
            {
                using var stream = product.ImageFile.OpenReadStream();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileByByte = memoryStream.ToArray();

                command.ImageFile = fileByByte;
            }

            command.ProductId = id;

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest(Response);
        }

        [MapToApiVersion("1.0")]
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult<Result>> DeleteProductAsync(Guid id)
        {
            var reponse = await _mediator.Send(new DeleteProductCommand() { ProductId = id});

            return reponse.IsSuccess ? Ok(reponse) : BadRequest(reponse);
        }
    }
}