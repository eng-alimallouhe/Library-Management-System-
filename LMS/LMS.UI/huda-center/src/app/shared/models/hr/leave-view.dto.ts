export interface LeaveOverviewDto{
    leaveId: string;
    employeeId: string;
    employeeName: string;
    startDate: Date;
    endDate: Date;
    leaveType: string;
    leaveStatus: string;
    isPaid: boolean;
}