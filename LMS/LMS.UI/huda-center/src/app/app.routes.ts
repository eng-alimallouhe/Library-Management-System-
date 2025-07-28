import { Routes } from '@angular/router';
import { EmployeesComponent } from './components/hr/employees/employees/employees.component';
import { MainLayoutComponent } from './layouts/main-layout/main-layout.component';
import { EmptyLayoutComponent } from './layouts/empty-layout/empty-layout.component';
import { LogInComponent } from './components/authentication/log-in/log-in.component';
import { ResetPasswordComponent } from './components/authentication/reset-password/reset-password.component';
import { VerifyComponent } from './components/authentication/verify/verify.component';
import { HR_ROUTES } from './components/hr/hr.routes';
import { RegisterComponent } from './components/authentication/register/register.component';
import { ProductsComponent } from './components/library-management/products/products/products.component';
import { PRODUCT_ROUTES } from './components/library-management/products/product.routs';

export const routes: Routes = [
  {
    path: '',
    component: ProductsComponent
  },
  {
    path: 'app',
    component: MainLayoutComponent,
    children: [
      { path: 'hr', children: HR_ROUTES },
      { path: 'products', children: PRODUCT_ROUTES }
    ]
  },
  {
    path: 'auth',
    component: EmptyLayoutComponent,
    children: [
      { path: 'login', component: LogInComponent },
      { path: 'reset-password', component: ResetPasswordComponent },
      { path: 'verify', component: VerifyComponent},
      { path: 'register', component: RegisterComponent}
    ]
  },
  { path: '**', redirectTo: 'dashboard' }
];