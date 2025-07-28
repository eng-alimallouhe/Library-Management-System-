import { Filter } from '../base/filter';


export enum PenaltyOrderBy {
    ByEmployeeName = 0,
    ByAmount = 1,
    ByReason = 2,
    ByDate = 3,
    ByIsApplied = 4,
    ByIsApproved = 5
}

export interface PenaltyFilter extends Filter {
    departmentIds?: string[];
    byIsApplied?: boolean | null;
    byIsApproved?: boolean | null;
    isDesc?: boolean;
    orderBy?: PenaltyOrderBy;
}
