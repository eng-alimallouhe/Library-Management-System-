import { AttendanceLookup } from "../../hr/attendance-view.dto";
import { RevenueOverviewDto } from "../../finacials/financial-revenue.dto";
import { IncentivesLookupDto } from "../../hr/inecntive-view.dto";
import { LeaveOverviewDto } from "../../hr/leave-view.dto";
import { PenaltyLookupDto } from "../../hr/penalty-view.dt";
import { SalariesOverviewDto } from "../../hr/salary-view.dto";
import { DepartmentHistoryDto } from "../departments/department-history.dto";

export interface EmployeeDetailsDto {
    employeeId: string;
    fullName: string;
    phoneNumber: string;
    hireDate: Date;
    email: string;
    baseSalary: number;
    currentDepartmentName: string;
    departmentsHistory: DepartmentHistoryDto[];
    employeeFinanical: RevenueOverviewDto[];
    attendances: AttendanceLookup[];
    incentives: IncentivesLookupDto[];
    penalties: PenaltyLookupDto[];
    leaves: LeaveOverviewDto[];
    salaries: SalariesOverviewDto[];
}