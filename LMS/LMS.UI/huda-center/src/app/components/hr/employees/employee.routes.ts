import { Routes } from "@angular/router";

export const EMPLOYEE_ROUTES : Routes = [
    {
        path: '',
        loadComponent: () => import('./employees/employees.component').then(m => m.EmployeesComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./employee/employee.component').then(m => m.EmployeeComponent)
    }
];