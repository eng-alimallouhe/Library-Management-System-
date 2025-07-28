import { Routes } from "@angular/router";

export const DEPSRTMENT_ROUTES : Routes = [
    {
        path: '',
        loadComponent: () => import('./departments/departments.component').then(m => m.DepartmentsComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./department/department.component').then(m => m.DepartmentComponent)
    }
]