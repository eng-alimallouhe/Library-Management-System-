using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.Customers;
using LMS.Domain.Customers.Models;

namespace LMS.Application.MappingProfiles.Customers.MappingsSettings
{
    public partial class CutomerToInactiveCustomerConverter : ITypeConverter<Customer, InActiveCustomersDto>
    {
        public InActiveCustomersDto Convert(Customer source, InActiveCustomersDto destination, ResolutionContext context)
        {
            var order = source.Orders.OrderByDescending(o => o.CreatedAt).FirstOrDefault();

            var lastOrderDate = DateTime.MinValue;

            if (order is not null)
                lastOrderDate = order.CreatedAt;

            return new InActiveCustomersDto
            {
                UserId = source.UserId,
                FullName = source.FullName,
                Posints = source.Points,
                Email = source.Email,
                CreatedAt = source.CreatedAt.ConvertToSyrianTime(),
                LastOrderDate = lastOrderDate.ConvertToSyrianTime()
            };
        }
    }
}