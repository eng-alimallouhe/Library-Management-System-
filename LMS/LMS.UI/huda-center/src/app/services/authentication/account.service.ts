import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { LoginRequestDto } from '../../shared/models/authentication/login-request.dto';
import { Observable } from 'rxjs';
import { AuthorizationDto } from '../../shared/models/authentication/authorization.dto';
import { ResetPassword } from '../../shared/models/authentication/reset-password.dto';
import { TResult } from '../../shared/models/results/t-result';

@Injectable({
  providedIn: 'root'
})
export class AccountService {

  apiUrl = environment.apiUrl + `authentication/Account`;

  constructor(
    private http: HttpClient
  ) { }


  Login(request: LoginRequestDto): Observable<TResult<AuthorizationDto>>{
    return this.http.post<TResult<AuthorizationDto>>(`${this.apiUrl}/log-in`, request);
  }

  CheackTowFactorAuthentication(email: string): Observable<TResult<AuthorizationDto>>{
    return this.http.get<TResult<AuthorizationDto>>(`${this.apiUrl}/tow-factor-verify/${email}`);
  }

  ResetPassword(request: ResetPassword): Observable<TResult<AuthorizationDto>>{
    return this.http.post<TResult<AuthorizationDto>>(`${this.apiUrl}/reset-password`, request);
  }
}