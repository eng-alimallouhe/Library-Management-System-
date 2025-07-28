import { Filter } from "../base/filter";

export interface EmployeeFilter extends Filter {
    departmentIds: string[]
}