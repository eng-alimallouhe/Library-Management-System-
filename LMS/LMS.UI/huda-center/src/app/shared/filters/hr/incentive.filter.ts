import { Filter } from '../base/filter';


export enum IncentiveOrderBy {
    ByEmployeeName = 0,
    ByDate = 1,
    ByReason = 2,
    ByIsPaid = 3,
    ByIsApproved = 4
}

export interface IncentiveFilter extends Filter {
    departmentIds?: string[];
    byIsPaid?: boolean | null;
    byIsApproved?: boolean | null;
    isDesc?: boolean;
    orderBy?: IncentiveOrderBy;
}
