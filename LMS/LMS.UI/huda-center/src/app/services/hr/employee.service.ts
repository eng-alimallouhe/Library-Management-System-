import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Result } from '../../shared/models/result/result';
import { TResult } from '../../shared/models/result/t-result';
import { TranslateService } from '@ngx-translate/core';
import { EmployeeAddingResponseDto } from '../../shared/models/hr/employee/employee-adding-response.dto';
import { EmployeeCreateRequestDto } from '../../shared/models/hr/employee/employee-create-request.dto';
import { EmployeeDetailsDto } from '../../shared/models/hr/employee/employee-details.dto';
import { EmployeeOverViewDto } from '../../shared/models/hr/employee/employee-overview.dto';
import { EmployeeTransferDto } from '../../shared/models/hr/employee/employee-transfer.dto';
import { EmployeeUpdateRequestDto } from '../../shared/models/hr/employee/employee-update-request.dto';
import { PagedResult } from '../../shared/models/results/paged-result.dto';
import { EmployeeFilter } from '../../shared/filters/hr/employee.filter';
import { QuerySupporterService } from '../../shared/services/query-supporter.service';
import { LanguageService } from '../../shared/services/language.service';
import { EmployeeSnapshotDto } from '../../shared/models/hr/employee/employee-snapshot.dto';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  apiUrl: string = environment.apiUrl + `hr/employee`;
  
  
  constructor(
    private http: HttpClient,
    private querySupporter: QuerySupporterService,
    private languageService: LanguageService
  ) { }

  getAllEmployees(filter: EmployeeFilter) : Observable<PagedResult<EmployeeOverViewDto>>{
    var queryParams = this.querySupporter.getEmployeesQueryParameters(filter);
    return this.http.get<PagedResult<EmployeeOverViewDto>>(`${this.apiUrl}`, {params: queryParams});
  }
  
  getEmployee(id: string) : Observable<EmployeeDetailsDto> {
    const language = this.languageService.getLanguageAsNumber();
    return this.http.get<EmployeeDetailsDto>(`${this.apiUrl}/${id}?language=${language}`);
  }
  
  getEmployeeSnapshot(employeeId: string) : Observable<EmployeeSnapshotDto>{
    return this.http.get<EmployeeSnapshotDto>(`${this.apiUrl}/snapshot/${employeeId}`);
  }

  addEmployee(request: EmployeeCreateRequestDto) : Observable<TResult<EmployeeAddingResponseDto>> {
    console.log(request);
    
    console.log('submit trigrred');
    const formData = new FormData();
    formData.append('fullName', request.fullName);
    formData.append('email', request.email);
    formData.append('phoneNumber', request.phoneNumber);
    formData.append('baseSalary', request.baseSalary.toString());
    formData.append('language', request.language.toString());
    formData.append('departmentId', request.departmentId);
    formData.append('appointmentDecisionFile', request.appointmentDecision);
    formData.append('FaceImage', request.FaceImage);
    return this.http.post<TResult<EmployeeAddingResponseDto>>(`${this.apiUrl}/`, formData);
  }


  transferEmployee(request: EmployeeTransferDto) : Observable<Result> {
    const formData = new FormData();
    formData.append('employeeId', request.employeeId);
    formData.append('departmentId', request.departmentId);
    formData.append('appointmentDecision', request.appointmentDecision);

    return this.http.put<Result>(`${this.apiUrl}/transfer-employee`, formData);  
  }


  updateEmployee(id: string, request: EmployeeUpdateRequestDto) : Observable<Result> {
    const formData = new FormData();
    formData.append('fullName', request.fullName);
    formData.append('email', request.email);
    formData.append('phoneNumber', request.phoneNumber);
    formData.append('baseSalary', request.baseSalary.toString());

    if (request.employeeFaceImage) {
      formData.append('employeeFaceImage', request.employeeFaceImage);
    }


    return this.http.put<Result>(`${this.apiUrl}/${id}`, formData); 
  }

  deleteEmployee(employeeId: string) : Observable<Result> {
    return this.http.delete<Result>(`${this.apiUrl}/${employeeId}`); 
  }
}
