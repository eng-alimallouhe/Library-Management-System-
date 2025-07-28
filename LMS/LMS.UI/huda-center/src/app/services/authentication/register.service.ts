import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AuthorizationDto } from '../../shared/models/authentication/authorization.dto';
import { RegisterDto } from '../../shared/models/authentication/register-dto';
import { Result } from '../../shared/models/results/result';
import { TResult } from '../../shared/models/results/t-result';

@Injectable({
  providedIn: 'root'
})
export class RegisterService {
  apiUrl : string = environment.apiUrl + "authentication/register";

  constructor(
    private http: HttpClient
  ) { }

  CheckUsernameAvailability(username: string) : Observable<Result>{
    return this.http.get<Result>(`${this.apiUrl}/username-availability/${username}`);
  }

  CreateTempAccount(registerDto: RegisterDto, file?: File): Observable<Result> {
    const formData = new FormData();
  
    formData.append('fullName', registerDto.fullName);
    formData.append('userName', registerDto.userName);
    formData.append('phoneNumber', registerDto.phoneNumber);
    formData.append('email', registerDto.email);
    formData.append('password', registerDto.password);
    formData.append('language', '0');
  
    if (file) {
      formData.append('profilePecture', file);
    }
  
    return this.http.post<Result>(`${this.apiUrl}/create-temp-account`, formData);
  }
  

  ActivateAccount(email: string) : Observable<TResult<AuthorizationDto>>{
    return this.http.get<TResult<AuthorizationDto>>(`${this.apiUrl}/activate-account/${email}`);
  }
}
