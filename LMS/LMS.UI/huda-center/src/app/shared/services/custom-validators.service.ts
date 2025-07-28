import { Injectable } from '@angular/core';
import { AbstractControl, AsyncValidatorFn, ValidationErrors } from '@angular/forms';
import { of, debounceTime, switchMap, map, catchError, Subject, takeUntil } from 'rxjs';
import { RegisterService } from '../../services/authentication/register.service';

@Injectable({
  providedIn: 'root'
})
export class CustomValidatorsService {

  constructor(
    private registerService: RegisterService
  ) { }


  
  passwordMatchValidator(control: AbstractControl): ValidationErrors | null {
    const password = control.get('password');
    const confirmPassword = control.get('confirmPassword');
  
    if (!password || !confirmPassword) return null;
  
    if (password.value !== confirmPassword.value) {
      confirmPassword.setErrors({ ...confirmPassword.errors, mismatch: true });
    } else {
      if (confirmPassword.hasError('mismatch')) {
        const errors = { ...confirmPassword.errors };
        delete errors['mismatch'];
        const hasOtherErrors = Object.keys(errors).length > 0;
        confirmPassword.setErrors(hasOtherErrors ? errors : null);
      }
    }  
    return null;
  }

  usernameAsyncValidator(destroy$: Subject<void>): AsyncValidatorFn {
    return (control: AbstractControl) => {
      if (!control.value) {
        return of(null);
      }
  
      return of(control.value).pipe(
        takeUntil(destroy$),
        debounceTime(500),
        switchMap(username =>
          this.registerService.CheckUsernameAvailability(username).pipe(
            map(() => null),
            catchError((error: any) => {
              if (error.status === 400 && error.error.statusKey === 'AUTHENTICATION.USER_NAME_UNAVALIABLE') {
                return of({ usernameTaken: true });
              }
              return of(null);
            })
          )
        )
      );
    };
  }

  
  noWhitespaceValidator(control: AbstractControl): ValidationErrors | null {
    const isWhitespace = (control.value || '').toString().includes(' ');
    return isWhitespace ? { whitespace: true } : null;
  }


  strongPasswordValidator(control: AbstractControl): ValidationErrors | null {
      const value = control.value || '';
    
      const hasMinLength = value.length >= 8; 
      const hasUpperCase = /[A-Z]/.test(value);
      const hasLowerCase = /[a-z]/.test(value);
      const hasNumber = /[0-9]/.test(value); 
    
      const valid = hasMinLength && hasUpperCase && hasLowerCase && hasNumber;
    
      return valid ? null : { weakPassword: true };
  }

  allowOnlyNumbers(event: KeyboardEvent): void {
    const allowedKeys = ['Backspace', 'ArrowLeft', 'ArrowRight', 'Tab'];
    const input = event.target as HTMLInputElement;
  
    if (
      allowedKeys.includes(event.key) ||
      /^[0-9]$/.test(event.key) && input.value.length < 6
    ) {
      return;
    }
  
    event.preventDefault();
  }


  getFirstErrorMessage(controlName: string, errors: ValidationErrors): string {
    const firstKey = Object.keys(errors)[0];
  
    const messages: { [key: string]: string } = {
      required: 'AUTHENTICATION.COMMON.THIS_FIELD_IS_REQUIRED',
      usernameTaken: 'AUTHENTICATION.COMMON.USER_NAME_UNAVALIABLE',
      mismatch: 'AUTHENTICATION.COMMON.NOT_MATCHING_PASSWORDS',
      whitespace: 'AUTHENTICATION.COMMON.THIS_FIELD_CANT_CONTAIN_WHITESPACE',
      weakPassword: 'AUTHENTICATION.COMMON.WEAK_PASSWORD',
      email: 'AUTHENTICATION.COMMON.INVALID_EMAIL',
      maxlength: 'AUTHENTICATION.COMMON.HIT_MAX_ATTEMPTS',
      minlength: 'AUTHENTICATION.COMMON.MUST_ENTER_MORE_CHARS',
    };

    return messages[firstKey] ?? 'UNKNOWN_ERROR';
  }
}
