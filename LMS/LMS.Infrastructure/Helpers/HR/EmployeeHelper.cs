using AutoMapper;
using LMS.Application.Abstractions.Authentication;
using LMS.Application.Abstractions.Common;
using LMS.Application.Abstractions.Communications;
using LMS.Application.Abstractions.HR;
using LMS.Application.Abstractions.Loggings;
using LMS.Application.Abstractions.Repositories;
using LMS.Application.Abstractions.UnitOfWorks;
using LMS.Application.DTOs.HR.Employees;
using LMS.Application.Filters.HR;
using LMS.Common.Enums;
using LMS.Common.Results;
using LMS.Domain.HR.Models;
using LMS.Domain.Identity.Models;
using LMS.Infrastructure.Specifications.HR.EmployeesDepartments;

namespace LMS.Infrastructure.Helpers.HR
{
    public class EmployeeHelper : IEmployeeHelper
    {
        private readonly ISoftDeletableRepository<EmployeeDepartment> _empDepRepo;
        private readonly ISoftDeletableRepository<Employee> _employeeRepo;
        private readonly ISoftDeletableRepository<Department> _departmentRepo;
        private readonly ISoftDeletableRepository<User> _userRepo;
        private readonly IFaceRecognitionHelper _faceRecognitionHelper;
        private readonly ISoftDeletableRepository<Role> _roleRepo;
        private readonly IFileHostingUploaderHelper _fileHostingUploaderHelper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConverterHelper _converterHelper;
        private readonly IAppLogger<EmployeeHelper> _logger;


        public EmployeeHelper(
            ISoftDeletableRepository<EmployeeDepartment> empDepRepo,
            ISoftDeletableRepository<Employee> employeeRepo,
            ISoftDeletableRepository<Department> departmentRepo,
            ISoftDeletableRepository<User> userRepo,
            IFaceRecognitionHelper faceRecognitionHelper,
            ISoftDeletableRepository<Role> roleRepo,
            IFileHostingUploaderHelper fileHostingUploaderHelper,
            IAppLogger<EmployeeHelper> logger,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IConverterHelper converterHelper)
        {
            _empDepRepo = empDepRepo;
            _departmentRepo = departmentRepo;
            _employeeRepo = employeeRepo;
            _userRepo = userRepo;
            _faceRecognitionHelper = faceRecognitionHelper;
            _roleRepo = roleRepo;
            _fileHostingUploaderHelper = fileHostingUploaderHelper;
            _unitOfWork = unitOfWork;
            _logger = logger;
            _mapper = mapper;
            _converterHelper = converterHelper;
        }


        // Assuming this is part of IEmployeeHelper implementation, e.g., EmployeeHelper.cs
        // You'll need to inject IUnitOfWork, ISoftDeletableRepository<Department>,
        // ISoftDeletableRepository<Role>, IFaceRecognitionHelper, IConverterHelper, IFileHostingUploaderHelper,
        // ISoftDeletableRepository<Employee>, ISoftDeletableRepository<EmployeeDepartment>, IAppLogger<T>
        // in its constructor.

        public async Task<Result<EmployeeCreatedDto>> CreateEmployee(
            Employee employee,
            Guid departmentId,
            byte[] appointment, // Assuming this is the raw PDF content
            byte[] faceImage)
        {
            // 1. Transaction Management (Moved from CommandHandler to here)
            // If this method manages the transaction, then CommandHandler should not.
            // If CommandHandler manages, remove Begin/Commit/Rollback from here.
            // I'll keep it as is, following your provided code's logic.
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // 2. Validate Department
                // Assuming GetByIdAsync by default fetches with tracking if you modify the entity later.
                // For read-only, explicitly use `false` for tracking. Here, it's read-only.
                var department = await _departmentRepo.GetByIdAsync(departmentId); // No tracking needed for read

                if (department is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // Rollback on validation failure
                    return Result<EmployeeCreatedDto>.Failure($"HR.DEPARTMENT.{ResponseStatus.DEPARTMENT_NOT_FOUNDED}");
                }

                // 3. Get Employee Role
                // No tracking needed for read.
                var employeesRole = await _roleRepo.GetByExpressionAsync(
                    role => role.RoleType.ToLower() == "employee", false); // No tracking needed for read

                if (employeesRole is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // Rollback on validation failure
                    return Result<EmployeeCreatedDto>.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");
                }

                // 4. Prepare Employee Data
                employee.IsEmailConfirmed = true; // Confirm email for new employees? This is a business decision.

                var password = GenerateRandomPassword(13);
                employee.HashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

                employee.UserName = await GenerateRandomUserName(employee.FullName); // This will access _userRepo

                // 5. Face Recognition
                var faceVector = _faceRecognitionHelper.ExtractFaceEncoding(faceImage);

                if (faceVector is null)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // Rollback on failure
                    return Result<EmployeeCreatedDto>.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}"); // More specific error?
                }

                employee.RoleId = employeesRole.RoleId;
                // employee.Role = employeesRole; // No need to set navigation property if RoleId is set and EF Core handles it.
                // This could even cause issues if employeesRole is already tracked.
                // It's safer to only set the FK (RoleId).

                employee.FaceFeatureVector = _converterHelper.ConvertDoubleArrayToBytes(faceVector);

                // 6. Add Employee to Repository
                // No explicit try-catch here, as the outer try-catch handles all exceptions for transaction.
                await _employeeRepo.AddAsync(employee);

                // 7. Upload Appointment Decision (PDF)
                var appointmentResult = await _fileHostingUploaderHelper.UploadPdfAsync(appointment, Guid.NewGuid().ToString());
                if (appointmentResult.IsFailed)
                {
                    await _unitOfWork.RollbackTransactionAsync(); // Rollback on failure
                                                                  // Log the specific failure reason from appointmentResult.StatusKey if possible.
                    _logger.LogError($"Failed to upload appointment decision for employee email: {employee.Email}. Status: {appointmentResult.StatusKey}", new Exception());
                    return Result<EmployeeCreatedDto>.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
                }

                // 8. Create Employee-Department Link
                var employeeDep = new EmployeeDepartment
                {
                    AppointmentDecisionUrl = appointmentResult.Value!,
                    EmployeeId = employee.UserId, // Ensure employee.UserId is populated after AddAsync or is generated by DB.
                                                  // If UserId is generated on Add, you might need to ensure it's accessible here.
                                                  // EF Core usually populates the ID after AddAsync when tracking.
                    DepartmentId = department.DepartmentId,
                    StartDate = DateTime.UtcNow,
                    IsActive = true // Assuming new assignment is active
                                    // You might also need to consider IsManager property based on the request.
                };

                // Add EmployeeDepartment to Repository
                // No explicit try-catch here, as the outer try-catch handles all exceptions for transaction.
                await _empDepRepo.AddAsync(employeeDep);

                // 9. Commit Transaction (All or nothing)
                await _unitOfWork.CommitTransactionAsync();

                // 10. Return success with generated credentials
                return Result<EmployeeCreatedDto>.Success(new EmployeeCreatedDto
                {
                    Email = employee.Email,
                    UserName = employee.UserName,
                    Password = password // This is the plain text password, careful with how it's handled.
                }, $"COMMON.{ResponseStatus.ADD_COMPLETED}");
            }
            catch (Exception ex)
            {
                // Catch-all for any unexpected errors during the process
                _logger.LogError($"An error occurred while creating employee for email: {employee.Email}", ex);
                await _unitOfWork.RollbackTransactionAsync();
                return Result<EmployeeCreatedDto>.Failure($"COMMON.{ResponseStatus.UNKNOWN_ERROR}");
            }
        }

        // في EmployeeHelper.cs (Implementation of IEmployeeHelper)
        // تأكد من حقن IAppLogger<T> في constructor الخاص بـ EmployeeHelper
        // public EmployeeHelper(..., IAppLogger<EmployeeHelper> logger) { _logger = logger; }

        public async Task<Result> TransferEmployeeAsync(Guid employeeId, Guid departmentId, byte[] appointment)
        {
            // لا يوجد await _unitOfWork.BeginTransactionAsync(); هنا، لأن CommandHandler هو من يدير المعاملة الآن

            // 1. Initial Validation
            if (appointment is null || appointment.Length == 0)
            {
                _logger.LogWarning($"Appointment decision file is missing or empty for employee transfer for EmployeeId: {employeeId}");
                return Result.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");
            }

            // 2. Fetch Department (read-only)
            var department = await _departmentRepo.GetByIdAsync(departmentId);

            if (department is null)
            {
                _logger.LogWarning($"Target department with ID {departmentId} not found for employee transfer.");
                return Result.Failure($"HR.DEPARTMENTS.{ResponseStatus.DEPARTMENT_NOT_FOUNDED}");
            }

            // 3. Fetch Employee (read-only)
            var employee = await _employeeRepo.GetByIdAsync(employeeId);

            if (employee is null)
            {
                _logger.LogWarning($"Employee with ID {employeeId} not found for transfer.");
                return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EMPLOYEE_NOT_FOUNDED}");
            }

            // 4. Get Current Employee-Department Link (with tracking)
            var oldEmpDep = await _empDepRepo.GetBySpecificationAsync(new EmployeeDepartmentByEmployeeIdSpecification(employeeId));

            if (oldEmpDep is not null)
            {
                if (oldEmpDep.DepartmentId == departmentId)
                {
                    _logger.LogInformation($"Employee {employeeId} is already assigned to department {oldEmpDep.DepartmentId}. No transfer needed.");
                    return Result.Failure($"HR.EMPLOYEES.{ResponseStatus.EXISTING_APPOINTMENT}");
                }

                oldEmpDep.IsActive = false;
                oldEmpDep.EndDate = DateTime.UtcNow;
            }
            else
            {
                _logger.LogWarning($"Employee {employeeId} does not have an active department assignment but is being transferred.");
            }

            // 5. Upload New Appointment Decision
            var uploadResult = await _fileHostingUploaderHelper.UploadPdfAsync(appointment, Guid.NewGuid().ToString());

            if (uploadResult.IsFailed)
            {
                // تعديل استخدام LogError هنا: رسالة ثم الاستثناء (إن وجد)
                // في هذه الحالة لا يوجد exception مباشر من uploadResult.IsFailed، لذا نستخدم LogError برسالة فقط.
                // إذا كان uploadResult.Error يحتوي على Exception، فيمكن تمريره.
                _logger.LogError($"Failed to upload appointment decision for employee transfer for EmployeeId: {employeeId}. Status: {uploadResult.StatusKey}", new Exception());
                return Result.Failure($"COMMON.{ResponseStatus.SOURCE_NOT_FOUND}");
            }

            // 6. Create New Employee-Department Link
            var employeeDep = new EmployeeDepartment
            {
                AppointmentDecisionUrl = uploadResult.Value!,
                EmployeeId = employeeId,
                DepartmentId = departmentId,
                StartDate = DateTime.UtcNow,
                IsActive = true,
                IsManager = false
            };

            await _empDepRepo.AddAsync(employeeDep);

            // لا يوجد commit هنا
            return Result.Success($"COMMON.{ResponseStatus.TASK_COMPLETED}");
        }


        public async Task<(ICollection<Employee> items, int count)> GetEmployeesPageAsync(EmployeeFilter filter)
        {
            var spec = new EmployeesFilteredSpecification(filter);

            var result = await _employeeRepo.GetAllAsync(spec);

            
            return result;
        }

        

        private async Task<string> GenerateRandomUserName(string cleanName)
        {
            cleanName = cleanName.Replace(" ", "_");

            var random = new Random();

            string finalResult = cleanName;

            bool isUnique = false;

            while (!isUnique)
            {
                finalResult = cleanName;

                for (int i = 0; i < 4; i++)
                {
                    var randomNuber = random.Next(65, 91);

                    var randomChar = (char)randomNuber;

                    finalResult += randomChar;
                }

                finalResult += random.Next(10, 99);

                var user = await _userRepo.GetByExpressionAsync(user => 
                user.UserName == finalResult, false);

                isUnique = user is null;
            }

            return finalResult;
        }

        private string GenerateRandomPassword(int passwordLength)
        {
            var finalResult = new char[passwordLength];

            var random = new Random();  

            var chars = new List<char>();

            chars.AddRange("abcdefghijklmnopqrstuvwxyz");
            chars.AddRange("ABCDEFGHIJKLMNOPQRSTUVWXYZ");
            chars.AddRange("0123456789");
            chars.AddRange("!@#$%^&*_~");

            for (int i = 0; i < passwordLength; i++)
            {
                finalResult[i] = chars[random.Next(chars.Count)];
            }

            return new string(finalResult);
        }

        public async Task<EmployeeDetailsDto?> BuildEmployeeDetailsAsync(Guid employeeId)
        {
            var spec = new EmployeeDetailsSpecification(employeeId);

            var employeeDetailsDto = await _employeeRepo.ProjectToEntityAsync<EmployeeDetailsDto>(spec, _mapper.ConfigurationProvider);

            return employeeDetailsDto;
        }

        public async Task<ICollection<Guid>> GetManagersIdsAsync()
        {
            var managerIds = new List<Guid>();

            var adminRole = await _roleRepo.GetByExpressionAsync(
                r => r.RoleType.ToLower().Contains("admin"), false);

            if (adminRole is not null)
            {
                var admins = await _userRepo.GetAllByExpressionAsync(u => u.RoleId == adminRole.RoleId);
                managerIds.AddRange(admins.Select(a => a.UserId));
            }

            var hrDepartment = await _departmentRepo.GetByExpressionAsync(
                d => d.DepartmentName.ToLower() == "hr", false);

            if (hrDepartment is not null)
            {
                var hrManager = await _empDepRepo.GetByExpressionAsync(
                    ed => ed.IsActive && ed.IsManager && ed.DepartmentId == hrDepartment.DepartmentId, false);

                if (hrManager is not null)
                {
                    managerIds.Add(hrManager.EmployeeId);
                }
            }

            return managerIds.Distinct().ToList();
        }
    }
}