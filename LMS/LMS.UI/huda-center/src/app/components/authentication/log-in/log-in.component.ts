import { CommonModule } from '@angular/common';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators, ValidationErrors } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AccountService } from '../../../services/authentication/account.service';
import { OtpService } from '../../../services/authentication/otp.service';
import { TokenService } from '../../../shared/services/token.service';
import { TResult } from '../../../shared/models/results/t-result';
import { AuthorizationDto } from '../../../shared/models/authentication/authorization.dto';
import { CustomValidatorsService } from '../../../shared/services/custom-validators.service';
import { AlterComponent } from "../../../shared/components/alter/alter.component";
import { LoaderComponent } from "../../../shared/components/loader/loader.component";

@Component({
  selector: 'app-log-in',
  standalone: true,
  imports: [
    TranslatePipe,
    ReactiveFormsModule,
    CommonModule,
    RouterModule,
    AlterComponent,
    LoaderComponent
],
  templateUrl: './log-in.component.html',
  styleUrl: './log-in.component.css'
})
export class LogInComponent implements OnInit, OnDestroy {
  loginForm!: FormGroup;
  isSubmited = false;
  isFiledAttempt = false;
  isLoading = false;
  email: string = '';

  failedAttempts = 0;
  maxAttempts = 3;
  accountLocked = false;

  alertVisible = false;
  alertMessage = '';

  constructor(
    private fb: FormBuilder,
    private translate: TranslateService,
    private accountService: AccountService,
    private otpService: OtpService,
    private router: Router,
    private tokenService: TokenService,
    public customValidatorsService: CustomValidatorsService
  ) {
  }
  ngOnDestroy(): void {
    console.log('log in destroyed');
  }

  ngOnInit(): void {
    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit(): void {
    if (this.accountLocked) {
      this.translate.get('AUTHENTICATION.COMMON.ACCOUNT_STILL_LOCKED').subscribe(msg => this.showAlert(msg));
      return;
    }

    if (this.loginForm.invalid) {
      this.isSubmited = true;
      return;
    }

    this.isSubmited = false;
    this.isLoading = true;

    this.email = this.loginForm.get('email')?.value || ' ';
    const data = this.loginForm.value;

    this.accountService.Login(data).subscribe({
      next: response => this.handleLoginSuccess(response),
      error: error => this.handleLoginError(error)
    });
  }

  private handleLoginSuccess(response: TResult<AuthorizationDto>): void {
    this.isLoading = false;
    
    const { accessToken, refreshToken } = response.value;

    this.tokenService.setTokens(accessToken, refreshToken);

    this.translate.get('AUTHENTICATION.LOGIN_SUCCESS').subscribe(msg => this.showAlert(msg));
    this.router.navigate(['/home']);
  }

  private handleLoginError(error: any): void {
    this.isLoading = false;
    const status : string = error?.error?.statusKey;
    
    if (status.includes('MORE_STEP_REQUERD')) {
      this.sendOTPFor2FA();
      return;
    }

    if (status.includes('FILED_ATTEMPT')) {
      this.failedAttempts++;

      if (this.failedAttempts >= this.maxAttempts) {
        this.accountLocked = true;
        this.translate.get(status).subscribe(msg => this.showAlert(msg));
      } else {
        const remaining = this.maxAttempts - this.failedAttempts;
        this.translate.get('AUTHENTICATION.COMMON.REMAINING_ATTEMPTS', { count: remaining }).subscribe(msg => this.showAlert(msg));
      }
      this.isFiledAttempt = true;
      return;
    }

    const errorKey = status || 'UNKNOWN_ERROR';
    this.accountLocked = true;
    this.translate.get(errorKey).subscribe(msg => this.showAlert(msg));
  }

  private sendOTPFor2FA(): void {
    const email = this.loginForm.get('email')?.value || '';

    this.otpService.SnedCode({ email, codeType: 0 }).subscribe({
      next: response => {
        this.isLoading = false;
        this.translate.get(response.statusKey).subscribe(msg => this.showAlert(msg));
        this.router.navigate(['/authentication/verify'], {
          queryParams: { email, codeType: 0 }
        });
      },
      error: otpError => {
        this.isLoading = false;
        const errorKey = otpError?.error?.statusKey || 'UNKNOWN_ERROR';
        this.translate.get(errorKey).subscribe(msg => this.showAlert(msg));
      }
    });
  }

  resetPassword(): void {
    if (!this.email || this.isLoading) return;

    this.isLoading = true;

    this.otpService.SnedCode({
      email: this.email,
      codeType: 2
    }).subscribe({
      next: response => {
        this.translate.get(response.statusKey).subscribe(msg => this.showAlert(msg));
        this.router.navigate(['/authentication/verify'], {
          queryParams: { email: this.email, codeType: 2 }
        });
        this.isLoading = false;
      },
      error: error => {
        const errorKey = error?.error?.statusKey || 'UNKNOWN_ERROR';
        this.translate.get(errorKey).subscribe(msg => this.showAlert(msg));
        this.isLoading = false;
      },
      complete: () => {
        this.isLoading = false;
      }
    });
  }

  showAlert(message: string): void {
    this.alertMessage = message;
    this.alertVisible = true;
  }
}
