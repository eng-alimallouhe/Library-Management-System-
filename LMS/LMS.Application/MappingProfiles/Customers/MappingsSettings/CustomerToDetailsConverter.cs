using AutoMapper;
using LMS.Application.Abstractions;
using LMS.Application.DTOs.Customers;
using LMS.Application.DTOs.Financial;
using LMS.Application.DTOs.Orders;
using LMS.Domain.Customers.Models;
using LMS.Domain.Identity.Enums;

namespace LMS.Application.MappingProfiles.Customers.MappingsSettings
{
    public partial class CustomerToDetailsConverter : ITypeConverter<Customer, CustomerDetailsDto>
    {
        public CustomerDetailsDto Convert(Customer source, CustomerDetailsDto destination, ResolutionContext context)
        {
            Language langEnum = Language.EN;

            if (context.Items.TryGetValue("lang", out var langObj) && langObj is int langInt)
            {
                langEnum = (Language)langInt;
            }

            var viewOrders = context.Mapper.Map<ICollection<OrderOverviewDto>>(source.Orders);
            
            var viewFinancial = context.Mapper.Map<ICollection<RevenueOverviewDto>>(source.Revenues);


            var levelName = source.Level.Translations.FirstOrDefault(lt => lt.Language == langEnum)?.LevelName ?? "N/A";

            return new CustomerDetailsDto
            {
                FullName = source.FullName,
                UserName = source.UserName,
                Email = source.Email,
                PhoneNumber = source.PhoneNumber,
                CreatedAt = source.CreatedAt.ConvertToSyrianTime(), 
                UpdatedAt = source.UpdatedAt.ConvertToSyrianTime(),
                LastLogIn = source.LastLogIn.ConvertToSyrianTime(),
                CurrentLevel = levelName,
                Points = source.Points,
                Revenues = viewFinancial,
                Orders = viewOrders,
                Addresses = source.Addresses,
            };
        }
    }
}