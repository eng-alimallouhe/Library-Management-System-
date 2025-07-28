import { Filter } from "../base/filter";

export interface DepartmentFilter extends Filter{
    maxEmployeeCount?: number | null;
    minEmployeeCount?: number | null;
}