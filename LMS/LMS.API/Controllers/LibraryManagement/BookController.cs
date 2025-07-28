using AutoMapper;
using LMS.API.DTOs.LibraryManagement.Books;
using LMS.Application.DTOs.Common;
using LMS.Application.DTOs.LibraryManagement.Books;
using LMS.Application.DTOs.LibraryManagement.Products;
using LMS.Application.Features.LibraryManagement.Books.Commands.AddBook;
using LMS.Application.Features.LibraryManagement.Books.Commands.UpdateBook;
using LMS.Application.Features.LibraryManagement.Books.Queries.GetAllBooks;
using LMS.Application.Features.LibraryManagement.Books.Queries.GetBookById;
using LMS.Application.Features.LibraryManagement.Books.Queries.GetBookToUpdate;
using LMS.Application.Filters.LibraryManagement;
using LMS.Common.Results;
using LMS.Domain.Identity.Enums;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace LMS.API.Controllers.LibraryManagement
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/stock/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public BookController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }


        [MapToApiVersion("1.0")]
        [HttpGet]
        public async Task<PagedResult<ProductOverviewDto>> GetAllBooksAsync([FromQuery] BooksFilter filter)
        {
            return await _mediator.Send(new GetAllBooksQuery(filter));
        }


        [MapToApiVersion("1.0")]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<BooksDetailsDto>?> GetBookByIdAsync(Guid id, [FromQuery]Language language)
        {
            var response = await _mediator.Send(new GetBookByIdQuery(language, id));

            return response != null? Ok(response) : NotFound();
        }


        [MapToApiVersion("1.0")]
        [HttpGet("to-update/{id:guid}")]
        public async Task<ActionResult<BooksDetailsDto>?> GetBookToUpdateAsync(Guid id, [FromQuery] Language language)
        {
            var response = await _mediator.Send(new GetBookToUpdateQuery(id, language));

            return response != null ? Ok(response) : NotFound();
        }

        [MapToApiVersion("1.0")]
        [HttpPost]
        public async Task<ActionResult<Result>> AddBookAsync([FromForm] BookCreateRequestDto book)
        {
            var command = _mapper.Map<AddBookCommand>(book);

            using var stream = book.ImageFile.OpenReadStream();
            var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var fileByByte = memoryStream.ToArray();

            command.ImageByte = fileByByte;

            var response = await _mediator.Send(command);

            return response.IsSuccess ? Ok(response) : BadRequest();
        }


        [MapToApiVersion("1.0")]
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Result>> UpdateBookAsync(Guid id, [FromForm] BookUpdateRequestDto book)
        {
            var command = _mapper.Map<UpdateBookCommand>(book);

            command.BookId = id;

            if (book.ImageFile != null)
            {
                using var stream = book.ImageFile.OpenReadStream();
                var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                var fileByByte = memoryStream.ToArray();

                command.ImageByte = fileByByte;
            }

            var response = await _mediator.Send(command);

            return response.IsSuccess? Ok(response) : BadRequest(response);
        }
    }
}
