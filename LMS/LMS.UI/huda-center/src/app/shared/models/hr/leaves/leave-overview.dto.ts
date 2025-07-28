import { LeaveStatus } from "./leave-status.enum";
import { LeaveType } from "./leave-type.enum";

export interface LeaveOverviewDto {
  leaveId: string; 
  employeeId: string;
  employeeName: string;
  startDate: string; 
  endDate: string; 
  leaveType: LeaveType;
  leaveStatus: LeaveStatus;
  isPaid: boolean;
}