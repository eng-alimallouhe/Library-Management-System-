import { Routes } from "@angular/router";

export const INCENTIVE_ROUTES : Routes = [
    {
        path: '',
        loadComponent: () => import('./incentives-list/incentives-list.component').then(m => m.IncentivesListComponent)
    },
    {
        path: 'all', 
        loadComponent: () => import('./incentives-list/incentives-list.component').then(m => m.IncentivesListComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./incentive-details/incentive-details.component').then(m => m.IncentiveDetailsComponent)
    }
];