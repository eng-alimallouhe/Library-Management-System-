import { Routes } from "@angular/router";
import { LogInComponent } from "./log-in/log-in.component";
import { RegisterComponent } from "./register/register.component";
import { ResetPasswordComponent } from "./reset-password/reset-password.component";
import { VerifyComponent } from "./verify/verify.component";

export const AUTHENTICATION_ROUTES : Routes = [
    {
        path: 'register',
        component: RegisterComponent
    },
    {
        path: 'verify',
        component: VerifyComponent
    },
    {
        path: 'login',
        component: LogInComponent
    },
    {
        path: 'reset-password',
        component: ResetPasswordComponent
    }
];