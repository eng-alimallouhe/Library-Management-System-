import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { Observable } from 'rxjs';
import { Result } from '../../shared/models/result/result';
import { DepartmentOverviewDto } from '../../shared/models/hr/departments/department-overview.dto';
import { PagedResult } from '../../shared/models/results/paged-result.dto';
import { DepartmenttoUpdateDto } from '../../shared/models/hr/departments/department-to-update.dto';
import { DepartmentDetailsDto } from '../../shared/models/hr/departments/departmentd-detail.dto';
import { DepartmentRequestDto } from '../../shared/models/hr/departments/department-request.dto';
import { AvaliableDepartmentDto } from '../../shared/models/hr/departments/avaliable-departments.dto';
import { DepartmentLookupDto } from '../../shared/models/hr/departments/department-lookup.dto';
import { QuerySupporterService } from '../../shared/services/query-supporter.service';
import { DepartmentFilter } from '../../shared/filters/hr/department.filter';

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {
  apiUrl: string = environment.apiUrl + `hr/department`;
  
  constructor(
    private http: HttpClient,
    private querySupporter: QuerySupporterService
  ) { }

  getAllDepartments(filter: DepartmentFilter) : Observable<PagedResult<DepartmentOverviewDto>>{
    var queryParams = this.querySupporter.getDepartmentsQueryParams(filter);

    return this.http.get<PagedResult<DepartmentOverviewDto>>(`${this.apiUrl}/`, {params: queryParams});
  }

  getDepartmentSnapshot(id: string) : Observable<DepartmenttoUpdateDto> {
    return this.http.get<DepartmenttoUpdateDto>(`${this.apiUrl}/snapshot/${id}`);  
  }

  getDepartment(id: string) : Observable<DepartmentDetailsDto> {
    return this.http.get<DepartmentDetailsDto>(`${this.apiUrl}/${id}`);
  }

  updateDepartment(id: string, request: DepartmentRequestDto) : Observable<Result> {
    return this.http.put<Result>(`${this.apiUrl}/${id}`, request);
  }

  addDepartment(request: DepartmentRequestDto) : Observable<Result> {
    return this.http.post<Result>(`${this.apiUrl}`, request);
  }

  getAvaliableDepartments(employeeId: string | null) : Observable<AvaliableDepartmentDto[]> {
    let finalUrl = `${this.apiUrl}/avaliable-departments`;

    if (employeeId != null) {
      finalUrl = `${finalUrl}?employeeId=${employeeId}`;
    }

    return this.http.get<AvaliableDepartmentDto[]>(`${finalUrl}`);
  }

  getDepartmentsLookup(name: string){
    return this.http.get<DepartmentLookupDto[]>(`${this.apiUrl}/${name}`);
  }

  getMaxEmployeeCount() : Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/max-count`);
  }

  deleteDepartment(departmentId: string){
    return this.http.delete<Result>(`${this.apiUrl}/${departmentId}`);
  }
}