import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormGroup, FormBuilder, Validators, ValidationErrors } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { RegisterService } from '../../../services/authentication/register.service';
import { OtpService } from '../../../services/authentication/otp.service';
import { CustomValidatorsService } from '../../../shared/services/custom-validators.service';
import { TokenService } from '../../../shared/services/token.service';
import { AccountService } from '../../../services/authentication/account.service';
import { AlterComponent } from "../../../shared/components/alter/alter.component";
import { Subject } from 'rxjs';
import { LoaderComponent } from "../../../shared/components/loader/loader.component";

@Component({
  selector: 'app-verify',
  imports: [
    TranslatePipe,
    CommonModule,
    ReactiveFormsModule,
    AlterComponent,
    LoaderComponent,
    RouterLink
],
  templateUrl: './verify.component.html',
  styleUrl: './verify.component.css'
})
export class VerifyComponent implements OnInit{
  
  verifyForm! : FormGroup;
  codeType: number = 1;
  email: string = '';
  isSubmited = false;
  isLoading = false;
  
  alertVisible = false;
  alertMessage = '';

  loaderWidth = '300px';

  constructor(
    private fb: FormBuilder,
    private otpService: OtpService,
    private route: ActivatedRoute,
    private router: Router,
    private registerService: RegisterService,
    private translate: TranslateService,
    private tokenService: TokenService,
    private accountService: AccountService,
    public customValidators: CustomValidatorsService
  ) {
  }

  ngOnInit(): void {
    const emailParam = this.route.snapshot.queryParamMap.get('email');
    this.email = emailParam ?? '';
  
    const codeTypeParam = this.route.snapshot.queryParamMap.get('codeType');

    const codeTypeTest = this.route.snapshot.queryParamMap.get('codeType');

    
    if (!codeTypeTest || parseInt(codeTypeTest) > 2 || parseInt(codeTypeTest) < 0) {
      this.showAlert('AUTHENTICATION.OTP_CODE.TYPE_ERROR');
      setTimeout(() => {
      }, 2000);
      return;
    }

    this.codeType = codeTypeParam ? parseInt(codeTypeParam) : 0;
  
    this.verifyForm = this.fb.group({
      email: [{ value: this.email, disabled: true }],
      code: ['', 
        [ Validators.required, 
          Validators.maxLength(6)
        ]]
    });
  }
  

  
  showAlert(message: string) {
    this.alertMessage = message;
    this.alertVisible = true;
  }

  onSubmit() {
    if (this.verifyForm.invalid) {
        this.isSubmited = true;
        return;
    }

    this.isSubmited = false;
    this.isLoading = true;

    this.otpService.Verify({
        email: this.email,
        code: this.verifyForm.get('code')?.value || '123456',
        codeType: this.codeType
    }).subscribe({
        next: (response) => {
            switch (this.codeType) {
                case 0:
                  this.handleTFA();
                  break;

                case 1:
                    this.activateAccount();
                    break;

                case 2:
                    this.handleNavigation('/authentication/reset-password', { email: this.email });
                    break;
                default:
                  var message = this.translate.instant('AUTHENTICATION.OTP_CODE.TYPE_ERROR'); 
            }
        },
        error: (err) => {
            this.handleError(err);
        }
    });
  }


  private activateAccount() {
      this.registerService.ActivateAccount(this.email).subscribe({
          next: response => {
            this.tokenService.setTokens(
              response.value.accessToken,
              response.value.refreshToken
            );
            this.showSuccessMessage('SUCCESS_VERIFY', '');
          },
          error: (err) => {
              this.handleError(err);
          }
      });
  }


  private handleNavigation(route: string, queryParams?: any) {
      this.isLoading = false;
      this.router.navigate([route], queryParams ? { queryParams } : undefined);
  }


  private showSuccessMessage(translationKey: string, navigateTo?: string) {
      this.isLoading = false;
      this.translate.get(translationKey).subscribe(translatedMessage => {
          this.showAlert(translatedMessage);
      });
  }


  private handleError(error: any) {
      this.isLoading = false;
      const translationKey = error?.error?.status || 'ERROR_UNKNOWN';
      this.translate.get(translationKey).subscribe(translatedMessage => {
          this.showAlert(translatedMessage);
      });
  }
 
  handleTFA() {
    this.accountService.CheackTowFactorAuthentication(this.email)
      .subscribe({
        next: response => {
          this.tokenService.setTokens(
            response.value.accessToken,
            response.value.refreshToken
          );
          this.handleNavigation('');
        },
        error: error => {
          this.handleError(error);
        }
      });
  }
  
}