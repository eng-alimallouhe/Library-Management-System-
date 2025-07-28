using AutoMapper;
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.DTOs.HR.Employees;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Models;
using MediatR;
using Microsoft.Extensions.Logging; // Add this for logging
using LMS.Application.Abstractions.UnitOfWorks; // Add this for UnitOfWork

namespace LMS.Application.Features.HR.Employees.Command.CreateEmployee
{
    public class CreateEmployeeCommandHandler : IRequestHandler<CreateEmployeeCommand, Result<EmployeeCreatedDto>>
    {
        private readonly IMapper _mapper;
        private readonly IEmployeeHelper _employeeHelper;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly IUnitOfWork _unitOfWork; // Add UnitOfWork
        private readonly ILogger<CreateEmployeeCommandHandler> _logger; // Add Logger

        public CreateEmployeeCommandHandler(
            IEmployeeHelper employeeHelper,
            ISoftDeletableRepository<User> userRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork, // Inject UnitOfWork
            ILogger<CreateEmployeeCommandHandler> logger) // Inject Logger
        {
            _employeeHelper = employeeHelper;
            _mapper = mapper;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork; // Initialize UnitOfWork
            _logger = logger; // Initialize Logger
        }

        async Task<Result<EmployeeCreatedDto>> IRequestHandler<CreateEmployeeCommand, Result<EmployeeCreatedDto>>.Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // 1. Check for existing user by email (without tracking)
            var existingUser = await _userRepo.GetByExpressionAsync(
                user => user.Email.ToLower().Trim() == request.Email.ToLower().Trim(), false); // Ensure no tracking

            if (existingUser is not null)
            {
                return Result<EmployeeCreatedDto>.Failure($"AUTHENTICATION.{ResponseStatus.EXISTING_ACCOUNT}");
            }

            // The Unit of Work transaction is now managed within CreateEmployee helper function,
            // which is a deviation from previous discussions where CommandHandler managed it.
            // Let's assume for this specific flow that IEmployeeHelper.CreateEmployee
            // handles the transaction as the provided code suggests.
            // If the preference is for CommandHandler to manage UoW, then CreateEmployee
            // should not call Begin/Commit/Rollback.

            // Given the provided `CreateEmployee` method *starts and commits* its own transaction,
            // this `CommandHandler` should *not* start its own.
            // This design choice centralizes the transaction logic within the helper, which can be acceptable
            // if `CreateEmployee` is designed as a standalone "transactional unit of work".

            // Mapping the employee before passing to helper.
            var employee = _mapper.Map<Employee>(request);

            // The helper method handles the full creation logic including transaction.
            var result = await _employeeHelper.CreateEmployee(employee, request.DepartmentId, request.AppointmentDecision, request.FaceImage);

            // No explicit Commit/Rollback here because the helper already did it.
            return result;
        }
    }
}