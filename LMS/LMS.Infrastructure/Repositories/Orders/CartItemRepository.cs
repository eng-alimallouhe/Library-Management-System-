using LMS.Domain.Orders.Models;
using LMS.Infrastructure.DbContexts;

namespace LMS.Infrastructure.Repositories.Orders
{
    public class CartItemRepository : BaseRepository<CartItem>
    {
        public CartItemRepository(LMSDbContext context) : base(context) { }
    }
}
