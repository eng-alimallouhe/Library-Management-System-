import { LeaveType } from "./leave-type.enum";

export interface LeaveUpdateRequestDto {
  employeeId: string;
  startDate: string; 
  endDate: string;
  leaveType: LeaveType;
  reason: string;
}