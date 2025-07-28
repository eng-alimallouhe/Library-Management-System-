using AutoMapper;
using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.UnitOfWorks; // إضافة هذا الاستيراد
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.Customers.Models; // تأكد أن هذا المسار صحيح لـ Customer
using MediatR;
using Microsoft.Extensions.Logging; // إضافة ILogger

namespace LMS.Application.Features.Authentication.Register.Commands.CreateTempAccount
{
    public class CreateTempAccountCommandHandler : IRequestHandler<CreateTempAccountCommand, Result>
    {
        private readonly IAuthenticationHelper _authenticationHelper;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork; // إضافة حقن IUnitOfWork
        private readonly ILogger<CreateTempAccountCommandHandler> _logger; // إضافة ILogger

        public CreateTempAccountCommandHandler(
            IAuthenticationHelper authenticationHelper,
            IMapper mapper,
            IUnitOfWork unitOfWork, // حقن IUnitOfWork
            ILogger<CreateTempAccountCommandHandler> logger) // حقن ILogger
        {
            _authenticationHelper = authenticationHelper;
            _mapper = mapper;
            _unitOfWork = unitOfWork; // تهيئة
            _logger = logger; // تهيئة
        }

        public async Task<Result> Handle(CreateTempAccountCommand request, CancellationToken cancellationToken)
        {
            // بدء معاملة لضمان ذرية عملية إنشاء الحساب
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var customer = _mapper.Map<Customer>(request);

                // دالة CreateAndSaveAccountAsync ستضيف الكيان للتتبع وتعدل بعض خصائصه.
                // لن تقوم بحفظ التغييرات بنفسها.
                var createAccountResult = await _authenticationHelper.CreateAndSaveAccountAsync(customer);

                if (createAccountResult.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // التراجع إذا فشل إنشاء الحساب
                    return createAccountResult;
                }

                // الالتزام بجميع التغييرات المعلقة (إضافة العميل الجديد)
                await _unitOfWork.CommitTransactionAsync();

                return createAccountResult; // سيعيد Result.Success من CreateAndSaveAccountAsync
            }
            catch (Exception ex)
            {
                // إذا حدث أي خطأ أثناء العملية بعد بدء المعاملة، نقوم بالتراجع
                _logger.LogError(ex, "An error occurred during temporary account creation for email: {Email}", request.Email);
                await _unitOfWork.RollbackTransactionAsync();
                return Result.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }
    }
}