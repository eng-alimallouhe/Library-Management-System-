using LMS.Application.Abstractions.Repositories;
using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Domain.HR.Models;
using LMS.Infrastructure.Specifications.HR.EmployeeIdentity;

public class EmployeeCompensationHelper : IEmployeeCompensationHelper
{
    private readonly ISoftDeletableRepository<Penalty> _penaltyRepo;
    private readonly ISoftDeletableRepository<Incentive> _incentiveRepo;
    private readonly ISoftDeletableRepository<Attendance> _attendanceRepo;
    private readonly ISoftDeletableRepository<Leave> _leaveRepo;


    public EmployeeCompensationHelper(
        ISoftDeletableRepository<Penalty> penaltyRepo,
        ISoftDeletableRepository<Incentive> incentiveRepo,
        ISoftDeletableRepository<Attendance> attendanceRepo,
        ISoftDeletableRepository<Leave> leaveRepo)
    {
        _penaltyRepo = penaltyRepo;
        _incentiveRepo = incentiveRepo;
        _attendanceRepo = attendanceRepo;
        _leaveRepo = leaveRepo;
    }

    public async Task<(ICollection<Penalty> items, int count)> GetPagedPenaltiesAsync(PenaltyFilter filter)
    {
        return await _penaltyRepo.GetAllAsync(new FilteredPenaltySpecification(filter));
    }


    public async Task<(ICollection<Attendance> items, int count)> GetPagedAttendancesAsync(AttendanceFilter filter)
    {
        return await _attendanceRepo.GetAllAsync(new FilteredAttendanceSpecification(filter));
    }

    public async Task<Penalty?> GetPenaltyByIdAsync(Guid penaltyId)
    {
        return await _penaltyRepo.GetBySpecificationAsync(new PenaltyDetailsSpecification(penaltyId));
    }


    public async Task<(ICollection<Incentive> items, int count)> GetPagedIncentivesAsync(IncentiveFilter filter)
    {
        var spec = new FilteredIncentiveSpecification(filter);
        return await _incentiveRepo.GetAllAsync(spec);
    }


    public async Task<Incentive?> GetIncentiveByIdAsync(Guid incentiveId)
    {
        return await _incentiveRepo.GetBySpecificationAsync(new IncentiveDetailsSpecification(incentiveId));
    }


    public async Task<(ICollection<Leave> items, int count)> GetFilteredLeavesAsync(LeaveFilter filter)
    {
        return await _leaveRepo.GetAllAsync(new FilteredLeaveSpecification(filter));
    }


    public async Task<Leave?> GetLeaveByIdAsync(Guid leaveId)
    {
        var spec = new LeaveDetailsSpecification(leaveId);
        return await _leaveRepo.GetBySpecificationAsync(spec);
    }
}
