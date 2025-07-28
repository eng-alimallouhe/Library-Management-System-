import { LeaveType } from "./leave-type.enum";

export interface LeaveCreateRequestDto {
  employeeId: string;
  startDate: string;
  endDate: string;
  leaveType: LeaveType;
  reason: string;
}
