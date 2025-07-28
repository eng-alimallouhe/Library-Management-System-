import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, ValidationErrors, Validators } from '@angular/forms';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { AccountService } from '../../../services/authentication/account.service';
import { ActivatedRoute, Router, RouterModule, RouterOutlet } from '@angular/router';
import { TokenService } from '../../../shared/services/token.service';
import { LoaderComponent } from "../../../shared/components/loader/loader.component";
import { CustomValidatorsService } from '../../../shared/services/custom-validators.service';

@Component({
  selector: 'app-reset-password',
  imports: [
    TranslatePipe,
    ReactiveFormsModule,
    CommonModule,
    LoaderComponent
],
  templateUrl: './reset-password.component.html',
  styleUrl: './reset-password.component.css'
})
export class ResetPasswordComponent implements OnInit {
  email = ' ';
  isLoading = false;
  resetFrom!: FormGroup;
  isSubmited = false;
  showPassword = false;
  showConfirmPassword = false;
  passwordHasMinLength = false;
  passwordHasNumber = false;
  passwordHasUpperCase = false;
  
  alertVisible = false;
  alertMessage = '';


  constructor(
    private translate: TranslateService,
    private tokenService: TokenService,
    private accountService: AccountService,
    private fb: FormBuilder,
    public router: Router,
    public route: ActivatedRoute,
    public customValidatorsService: CustomValidatorsService
  ) { 
  }


  ngOnInit(): void {
    const emailParam = this.route.snapshot.queryParamMap.get('email');
    this.email = emailParam? emailParam : ' ';

    this.resetFrom = this.fb.group({
      password: ['', [
        Validators.required, 
        this.customValidatorsService.strongPasswordValidator
      ]], 
      confirmPassword: ['', [
        Validators.required
      ]]
    }, { validators: this.customValidatorsService.passwordMatchValidator });
  }


  onSubmit(){
    if (this.resetFrom.invalid) {
      this.isLoading = false;
      this.isSubmited = true;
    }

    this.isLoading = true;
    this.isSubmited = false;
    
    this.accountService.ResetPassword({
      newPassword: this.resetFrom.get('password')?.value || ' ',
      email: this.email
    }).subscribe({
      next: response => {
        this.tokenService.setTokens(
          response.value.accessToken,
          response.value.refreshToken
        );

        this.showSuccessMessage(response.statusKey, '/home');

      },
      error: error => {
        this.handleError(error);
      }
    });
  }

  togglePassword() {
    this.showPassword = !this.showPassword;
  }
  
  toggleConfirmPassword() {
    this.showConfirmPassword = !this.showConfirmPassword;
  }
  
  updatePasswordRules() {
    const password = this.resetFrom.get('password')?.value || '';
    this.passwordHasMinLength = password.length >= 8;
    this.passwordHasNumber = /\d/.test(password);
    this.passwordHasUpperCase = /[A-Z]/.test(password);
  }

  private showSuccessMessage(translationKey: string, navigateTo?: string) {
    this.isLoading = false;
    this.translate.get(translationKey).subscribe(message => {
        this.showAlert(message);
        setTimeout(() => {
          if (navigateTo) {
            this.router.navigate([navigateTo]);
          }
        }, 2000);
    });
  }

  private handleError(error: any) {
    this.isLoading = false;
    const translationKey = error?.error?.status || 'ERROR_UNKNOWN';
    this.translate.get(translationKey).subscribe(message => {
        this.showAlert(message);
    });
  }

  
  showAlert(message: string): void {
    this.alertMessage = message;
    this.alertVisible = true;
  }
}