using AutoMapper;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using MediatR;
using Microsoft.Extensions.Logging; // إضافة هذا الاستيراد للتسجيل

namespace LMS.Application.Features.HR.Departments.Command.CreateDepartment
{
    public class CreateDepartmentCommandHandler : IRequestHandler<CreateDepartmentCommand, Result>
    {
        private readonly ISoftDeletableRepository<Department> _departmentRepo;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork; // حقن IUnitOfWork
        private readonly ILogger<CreateDepartmentCommandHandler> _logger; // حقن ILogger

        public CreateDepartmentCommandHandler(
            ISoftDeletableRepository<Department> departmentRepo,
            IMapper mapper,
            IUnitOfWork unitOfWork, // إضافة IUnitOfWork إلى constructor
            ILogger<CreateDepartmentCommandHandler> logger) // إضافة ILogger إلى constructor
        {
            _departmentRepo = departmentRepo;
            _mapper = mapper;
            _unitOfWork = unitOfWork; // تهيئة IUnitOfWork
            _logger = logger; // تهيئة ILogger
        }

        public async Task<Result> Handle(CreateDepartmentCommand request, CancellationToken cancellationToken)
        {
            // بدء معاملة لضمان ذرية عملية إنشاء القسم
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 1. التحقق من عدم وجود قسم بنفس الاسم لتجنب التكرار
                // نحتاج إلى الوصول إلى DepartmentName من الـ request
                // سنفترض أن CreateDepartmentCommand يحتوي على خاصية DepartmentName
                var existingDepartment = await _departmentRepo.GetByExpressionAsync(
                    d => d.DepartmentName.ToLower().Trim() == request.DepartmentName.ToLower().Trim(), false); // <--- تعطيل التتبع

                if (existingDepartment is not null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع في حالة وجود تكرار
                    return Result.Failure($"COMMON.{ResponseStatus.ALREADE_EXIST_RECORD}");
                }

                // 2. تعيين Command إلى كيان Department
                var newDepartment = _mapper.Map<Department>(request);

                // 3. إضافة الكيان إلى المستودع (سيتم تتبعه بواسطة EF Core)
                await _departmentRepo.AddAsync(newDepartment);

                // 4. الالتزام بجميع التغييرات المعلقة (إضافة القسم الجديد)
                await _unitOfWork.CommitTransactionAsync();

                return Result.Success($"COMMON.{ResponseStatus.ADD_COMPLETED}");
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع
                _logger.LogError(ex, "Error creating new department: {DepartmentName}", request.DepartmentName);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}