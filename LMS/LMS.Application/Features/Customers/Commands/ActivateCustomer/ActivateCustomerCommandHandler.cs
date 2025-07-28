using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Customers.Models;
using MediatR;

namespace LMS.Application.Features.Customers.Commands.ActivateCustomer
{
    public class ActivateCustomerCommandHandler : IRequestHandler<ActivateCustomerCommand, Result>
    {
        private readonly ISoftDeletableRepository<Customer> _customerRepo;
        private readonly IAppLogger<ActivateCustomerCommandHandler> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public ActivateCustomerCommandHandler(
            ISoftDeletableRepository<Customer> customerRepo,
            IUnitOfWork unitOfWork,
            IAppLogger<ActivateCustomerCommandHandler> logger)
        {
            _customerRepo = customerRepo;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public async Task<Result> Handle(ActivateCustomerCommand request, CancellationToken cancellationToken)
        {
            var customer = await _customerRepo.GetByIdAsync(request.CustomerId);

            if (customer is null)
            {
                return Result.Failure($"COMMON.{ResponseStatus.RECORD_NOT_FOUND}");
            }

            if (!customer.IsDeleted)
            {
                return Result.Failure($"COMMON.{ResponseStatus.STATUS_ALREADY_SET}");
            }

            customer.IsDeleted = false;

            try
            {
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                return Result.Success($"COMMON.{ResponseStatus.UPDATE_COMPLETED}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error While updating the status for account : {customer.UserName}", ex);
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}
