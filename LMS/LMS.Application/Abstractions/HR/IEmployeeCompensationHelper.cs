using LMS.Application.Filters.HR.EmployeeIdentity;
using LMS.Domain.HR.Models;

public interface IEmployeeCompensationHelper
{
    Task<(ICollection<Penalty> items, int count)> GetPagedPenaltiesAsync(PenaltyFilter filter);
    Task<Penalty?> GetPenaltyByIdAsync(Guid penaltyId);

    Task<(ICollection<Incentive> items, int count)> GetPagedIncentivesAsync(IncentiveFilter filter);
    Task<Incentive?> GetIncentiveByIdAsync(Guid incentiveId);

    Task<(ICollection<Leave> items, int count)> GetFilteredLeavesAsync(LeaveFilter filter);

    Task<(ICollection<Attendance> items, int count)> GetPagedAttendancesAsync(AttendanceFilter filter);
    
    Task<Leave?> GetLeaveByIdAsync(Guid leaveId);
}
