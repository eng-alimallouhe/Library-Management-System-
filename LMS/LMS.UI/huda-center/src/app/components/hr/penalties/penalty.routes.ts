import { Routes } from "@angular/router";

export const PENALTY_ROUTES : Routes = [
    {
        path: '',
        loadComponent: () => import('./penalties/penalties.component').then(m => m.PenaltiesComponent)
    },
    {
        path: 'all',
        loadComponent: () => import('./penalties/penalties.component').then(m => m.PenaltiesComponent)
    },
    {
        path: ':id',
        loadComponent: () => import('./penalty/penalty.component').then(m => m.PenaltyComponent)
    }
];