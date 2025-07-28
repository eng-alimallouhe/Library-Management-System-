// src/app/shared/filters/hr/leave-filter.ts

import { LeaveStatus } from '../../models/hr/leaves/leave-status.enum';
import { LeaveType } from '../../models/hr/leaves/leave-type.enum';
import { Filter } from '../base/filter'; 
export enum LeaveOrderBy {
  ByEmployeeName = 0,
  ByStartDate = 1,
  ByEndDate = 2,
  ByStatus = 3,
  ByType = 4,
  ByCreatedAt = 5
}

export interface LeaveFilter extends Filter {
  status?: LeaveStatus | null; // int? في الباك إند
  type?: LeaveType | null; // int? في الباك إند
  isPaid?: boolean | null;
  isDesc?: boolean;
  orderBy?: LeaveOrderBy;
}
