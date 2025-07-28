import { Routes } from "@angular/router";
import { DashboardComponent } from "./dashboard/dashboard.component";

export const ADMIN_ROUTES : Routes = [
    {
        path: '',
        component: DashboardComponent 
    },
    {
        path: 'dashboard',
        loadComponent: () => import('./dashboard/dashboard.component').then(m => m.DashboardComponent)
    }
];