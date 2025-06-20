namespace LMS.Domain.Identity.ValueObjects
{
    public class Address
    {
        //Primary Key:
        public Guid AddressId { get; set; }


        //Foreign Key: RoleId ==> one(user)-to-many(address) relationship
        public Guid CustomerId { get; set; }

        public required string Latitude { get; set; }
        public required string Longitude { get; set; }
        public required string Governorate { get; set; }
        public required string City { get; set; }
        public required string Details { get; set; }
        public required string PhoneNumber { get; set; }


        public Address()
        {
            AddressId = Guid.NewGuid();
        }
    }
}
