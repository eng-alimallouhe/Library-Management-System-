import { Routes } from "@angular/router";

export const LEAVE_ROUTES : Routes = [
    {
        path: '',
        loadComponent: () => import('./leaves-list/leaves-list.component').then(m => m.LeavesListComponent)
    },
    {
        path: 'all',
        loadComponent: () => import('./leaves-list/leaves-list.component').then(m => m.LeavesListComponent)
    },
    {
        path: 'history',
        loadComponent: () => import('./employee-leave-history/employee-leave-history.component').then(m => m.EmployeeLeavesHistoryComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./leaves-details/leaves-details.component').then(m => m.LeaveDetailsComponent)
    }
];