using LMS.Domain.Identity.ValueObjects;

namespace LMS.Domain.Orders.Models
{
    public class Shipment
    {
        //Foreign Key: AddressId ==> one(Address)-to-one(PrintOrder) relationship
        public Guid AddressId { get; set; }
        
        
        //Foreign Key: AddressId ==> one(Address)-to-one(PrintOrder) relationship
        public Guid OrderId { get; set; }

        public decimal Cost { get; set; }

        public Address Address { get; set; }
        public Order Order { get; set; }


        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        public Shipment()
        {
            Address = null!;
            Order = null!;
        }
    }
}