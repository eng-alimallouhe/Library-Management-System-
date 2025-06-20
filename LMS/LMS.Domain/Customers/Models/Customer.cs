using LMS.Domain.Customers.Models.Levels;
using LMS.Domain.Financial.Models;
using LMS.Domain.Identity.Models;
using LMS.Domain.Identity.ValueObjects;
using LMS.Domain.Orders.Models;

namespace LMS.Domain.Customers.Models
{
    public class Customer : User
    {
        //Foreign Key: LevelId ==> one(customer)-to-one(level) relationship
        public Guid LevelId { get; set; }


        public decimal Points { get; set; }


        //navigation property:
        public LoyaltyLevel Level { get; set; }
        public ICollection<Address> Addresses { get; set; } 
        public Cart Cart { get; set; } 
        public ICollection<BaseOrder> Orders { get; set; } 
        public ICollection<Revenue> FinancialRevenues { get; set; }


        public Customer()
        {
            LevelId = Guid.NewGuid();
            Level = null!;
            Cart = null!;
            Points = 0;
            Addresses = [];
            Orders = [];
            FinancialRevenues = [];
        }

    }
}
