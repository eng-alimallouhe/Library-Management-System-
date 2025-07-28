using LMS.Common.Results;
using MediatR;

namespace LMS.Application.Features.LibraryManagement.Products.Commands.AddProduct
{
    public class AddProductCommand : IRequest<Result>
    {
        public string ARProductName { get; set; }
        public string ARProductDescription { get; set; }
        public string ENProductName { get; set; }
        public string ENProductDescription { get; set; }
        public decimal ProductPrice { get; set; }
        public int ProductStock { get; set; }
        public ICollection<Guid> CategoriesIds { get; set; } = [];
        public byte[] ImageFile { get; set; } = new byte[0];
    }
}
