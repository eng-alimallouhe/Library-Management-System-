import { LeaveStatus } from "./leave-status.enum";
import { LeaveType } from "./leave-type.enum";

export interface LeaveDetailsDto {
  leaveId: string;
  employeeId: string;
  employeeName: string;
  startDate: Date;
  endDate: Date;
  leaveType: LeaveType;
  leaveStatus: LeaveStatus;
  isPaid: boolean;
  reason: string;
  createdAt: Date; 
  updatedAt: Date;
}