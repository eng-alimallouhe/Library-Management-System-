import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators, AbstractControl, ValidationErrors, ReactiveFormsModule, AsyncValidatorFn } from '@angular/forms';
import { catchError, debounceTime, finalize, map, Observable, of, Subject, switchMap } from 'rxjs';
import { Router, RouterModule } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { CommonModule } from '@angular/common';
import { Result } from '../../../shared/models/results/result';
import { RegisterService } from '../../../services/authentication/register.service';
import { OtpService } from '../../../services/authentication/otp.service';
import { CustomValidatorsService } from '../../../shared/services/custom-validators.service';
import { LoaderComponent } from '../../../shared/components/loader/loader.component';
import { AlterComponent } from "../../../shared/components/alter/alter.component";



@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    TranslatePipe,
    CommonModule,
    RouterModule,
    LoaderComponent,
    AlterComponent
],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent  implements OnInit, OnDestroy{
  registerForm!: FormGroup;
  codeSend$!: Observable<Result>;
  isSubmited = false;
  isLoading: boolean = false;

  alertVisible = false;
  alertMessage = '';

  showAlert(message: string) {
    this.alertMessage = message;
    this.alertVisible = true;
  }

  
  private destroy$ = new Subject<void>();


  constructor(
    private router: Router,
    private fb: FormBuilder,
    private registerService: RegisterService,
    private otpService: OtpService,
    private translate: TranslateService,
    public validators: CustomValidatorsService,
  ) { 
  }
  

  ngOnInit(): void {
    this.registerForm = this.fb.group({
      fullName: ['', [
        Validators.required,
        Validators.maxLength(60)
      ]],
  
      userName: ['', {
        validators: [
          Validators.required,
          this.validators.noWhitespaceValidator.bind(this),
          Validators.maxLength(60)
        ],
        asyncValidators: [this.validators.usernameAsyncValidator(this.destroy$)],
        updateOn: 'change' 
      }],
  
      phoneNumber: ['', [
        Validators.required,
        Validators.maxLength(23)
      ]],
  
      email: ['', [
        Validators.required,
        Validators.email
      ]],
  
      password: ['', [
        Validators.required,
        Validators.maxLength(60),
        this.validators.strongPasswordValidator
      ]],
  
      confirmPassword: ['', Validators.required,]
    }, { validators: this.validators.passwordMatchValidator });
  }
  

  onSubmit() {
    console.log('submit triggered');
    
    if (this.registerForm.valid) {
      this.isLoading = true;
      
      this.isSubmited = false;
      const data = this.registerForm.value;
      
      this.registerService
      .CreateTempAccount(data)
      .subscribe({
        next: response => {
          this.sendotpCode(data.email, 1)
        },
        error: error => {
          this.isLoading = false;
          const key = error.error.statusKey;
          let message = '';
          this.translate.get(key)
          .subscribe(translatedMessage => {
            message = translatedMessage;
            this.showAlert(message);
          });
        }
      });
    }

    else {
      this.isSubmited = true;
      console.error("error");
    }
  }

  
  sendotpCode(email: string, codeType: number){
    this.otpService.SnedCode({
      codeType: codeType,
      email: email
    })
    .pipe(finalize(() => {
      this.isLoading = false;
    }))
    .subscribe({
      next: response => {
        console.log(response.statusKey);
        const key = response.statusKey;
        this.translate.get(key)
        .subscribe(translatedMessage => {
          this.showAlert(translatedMessage);
        });
        setTimeout(() => {
          this.router.navigate(['/verify-otp'], {
            queryParams: {
              email: email,
              type: codeType
            }
          });
        }, 2000);

      }, 
      error: error => {
        this.translate.get(error.error.statusKey)
        .subscribe(translatedMessage => {
          this.showAlert(translatedMessage);
        });
      }
    });
  }


  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

}
