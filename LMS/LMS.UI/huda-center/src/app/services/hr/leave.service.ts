// src/app/services/hr/leave.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment'; // تأكد من وجود ملف البيئة

// استيراد الـ DTOs والفلاتر
import { PagedResult } from '../../shared/models/results/paged-result.dto';
import { LeaveFilter } from '../../shared/filters/hr/leave.filter';
import { LeaveOverviewDto } from '../../shared/models/hr/leave-view.dto';
import { LeaveDetailsDto } from '../../shared/models/hr/leaves/leave-details.dto';
import { LeaveUpdateRequestDto } from '../../shared/models/hr/leaves/leave-update-request.dto';
import { LeaveCreateRequestDto } from '../../shared/models/hr/leaves/leaves-create-request.dto.ts';
import { Result } from '../../shared/models/result/result';

@Injectable({
  providedIn: 'root'
})
export class LeaveService {
  private baseUrl = environment.apiUrl + 'hr/Leave'; // تأكد من أن apiUrl يشير إلى مسار الـ API الصحيح

  constructor(private http: HttpClient) { }

  /**
   * @description Fetches a paginated list of all leave requests based on provided filters.
   * @param filter The LeaveFilter object containing pagination and filtering criteria.
   * @returns An Observable of PagedResult<LeaveOverviewDto>.
   */
  getAllLeaves(filter: LeaveFilter): Observable<PagedResult<LeaveOverviewDto>> {
    let params = new HttpParams();
    if (filter.pageNumber) params = params.append('pageNumber', filter.pageNumber.toString());
    if (filter.pageSize) params = params.append('pageSize', filter.pageSize.toString());
    if (filter.name) params = params.append('name', filter.name);
    if (filter.from) params = params.append('from', filter.from.toString());
    if (filter.to) params = params.append('to', filter.to.toString());
    if (filter.status !== null && filter.status !== undefined) params = params.append('status', filter.status.toString());
    if (filter.type !== null && filter.type !== undefined) params = params.append('type', filter.type.toString());
    if (filter.isPaid !== null && filter.isPaid !== undefined) params = params.append('isPaid', filter.isPaid.toString());
    if (filter.isDesc !== null && filter.isDesc !== undefined) params = params.append('isDesc', filter.isDesc.toString());
    if (filter.orderBy !== null && filter.orderBy !== undefined) params = params.append('orderBy', filter.orderBy.toString());

    return this.http.get<PagedResult<LeaveOverviewDto>>(`${this.baseUrl}`, { params });
  }

  getEmployeeLeaves(employeeId: string): Observable<PagedResult<LeaveOverviewDto>> {
    let params = new HttpParams();
    params.append('employeeId', employeeId);
    params.append('pageNumber', 1);
    params.append('pageSize', 1000);
    
    return this.http.get<PagedResult<LeaveOverviewDto>>(`${this.baseUrl}?employeeId=${employeeId}&pageNumber=1&pageSize=100000`);
  }

  /**
   * @description Fetches details of a specific leave request by its ID.
   * @param id The GUID of the leave request.
   * @returns An Observable of LeaveDetailsDto.
   */
  getLeaveById(id: string): Observable<LeaveDetailsDto> {
    return this.http.get<LeaveDetailsDto>(`${this.baseUrl}/${id}`);
  }

  /**
   * @description Creates a new leave request.
   * @param request The LeaveCreateRequestDto containing the new leave details.
   * @returns An Observable of Result.
   */
  createLeave(request: LeaveCreateRequestDto): Observable<Result> {
    // بما أن الباك إند يستخدم [FromForm] لـ Update، سنفترض أنه يستخدم JSON لـ Create
    // إذا كان يستخدم [FromForm] هنا أيضاً، يجب تعديل هذا الجزء
    return this.http.post<Result>(`${this.baseUrl}`, request);
  }

  /**
   * @description Updates an existing leave request.
   * @param id The GUID of the leave request to update.
   * @param request The LeaveUpdateRequestDto containing the updated leave details.
   * @returns An Observable of Result.
   */
  updateLeave(id: string, request: LeaveUpdateRequestDto): Observable<Result> {
    // بما أن الباك إند يستخدم [FromForm] لـ Update، يجب بناء FormData
    const formData = new FormData();
    formData.append('employeeId', request.employeeId);
    formData.append('startDate', request.startDate);
    formData.append('endDate', request.endDate);
    formData.append('leaveType', request.leaveType.toString());
    formData.append('reason', request.reason);
    // إذا كان هناك ملف قرار في المستقبل، يمكن إضافته هنا:
    // if (request.decisionFile) { formData.append('decisionFile', request.decisionFile); }

    return this.http.put<Result>(`${this.baseUrl}/${id}`, formData);
  }

  /**
   * @description Approves or rejects a leave request.
   * @param id The GUID of the leave request.
   * @param newStatus The new status (true for Approved, false for Rejected).
   * @returns An Observable of Result.
   */
  approveRejectLeave(id: string, isApproved: boolean): Observable<Result> {
    return this.http.put<Result>(`${this.baseUrl}/approve/${id}?isAproved=${isApproved}`, {});
  }

  /**
   * @description Deletes a leave request by its ID.
   * @param id The GUID of the leave request to delete.
   * @returns An Observable of Result.
   */
  deleteLeave(id: string): Observable<Result> {
    return this.http.delete<Result>(`${this.baseUrl}/${id}`);
  }
}
