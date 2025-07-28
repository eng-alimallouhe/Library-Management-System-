import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { OtpCodeSendRequstDTO } from '../../shared/models/authentication/otp-code-request.dto';
import { Result } from '../../shared/models/results/result';
import { Observable } from 'rxjs';
import { OtpVerifyRequest } from '../../shared/models/authentication/otp-verify-request.dto';

@Injectable({
  providedIn: 'root'
})
export class OtpService {
  apiUrl: string = environment.apiUrl + "authentication/OTPCode";

  constructor(private http: HttpClient) { }

  SnedCode(request: OtpCodeSendRequstDTO) : Observable<Result>{
    return this.http.post<Result>(`${this.apiUrl}/send-otpcode`, request);
  }

  Verify(request: OtpVerifyRequest) : Observable<Result>{
    return this.http.post<Result>(`${this.apiUrl}/verify`, request);
  }
}
