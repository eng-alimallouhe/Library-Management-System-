import { Routes } from "@angular/router";
import { EMPLOYEE_ROUTES } from "./employees/employee.routes";
import { DEPSRTMENT_ROUTES } from "./departments/department.routes";
import { PENALTY_ROUTES } from "./penalties/penalty.routes";
import { INCENTIVE_ROUTES } from "./incentives/incentive.routes";
import { LEAVE_ROUTES } from "./leaves/leave.routes";

export const HR_ROUTES : Routes = [
    {
        path: '',
        loadComponent: () => import('./employees/employees/employees.component').then(m => m.EmployeesComponent)
    },
    {
        path: 'employees',
        children: EMPLOYEE_ROUTES
    },
    {
        path: 'departments', 
        children: DEPSRTMENT_ROUTES
    },
    {
        path: 'penalties',
        children: PENALTY_ROUTES
    },
    {
        path: 'incentives',
        children: INCENTIVE_ROUTES
    },
    {
        path: 'leaves',
        children: LEAVE_ROUTES
    }
];