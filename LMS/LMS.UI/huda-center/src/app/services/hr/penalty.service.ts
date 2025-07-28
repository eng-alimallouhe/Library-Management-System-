// src/app/services/hr/penalty.service.ts

import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

// استيراد الـ DTOs والفلاتر والـ Result
import { PagedResult } from '../../shared/models/results/paged-result.dto';
import { PenaltyOverviewDto } from '../../shared/models/hr/penalties/penalty-overview.dto';
import { PenaltyDetailsDto } from '../../shared/models/hr/penalties/penalty-details.dto';
import { PenaltyCreateRequestDto } from '../../shared/models/hr/penalties/penalty-create-request.dto';
import { PenaltyUpdateRequestDto } from '../../shared/models/hr/penalties/penalty-update-request.dto';
import { PenaltyFilter } from '../../shared/filters/hr/penalty-filter';
import { AvaliableDepartmentDto } from '../../shared/models/hr/departments/avaliable-departments.dto'; // لاستيراد الأقسام المتاحة
import { Result } from '../../shared/models/result/result';
import { environment } from '../../../environments/environment';
import { QuerySupporterService } from '../../shared/services/query-supporter.service';

@Injectable({
  providedIn: 'root'
})
export class PenaltyService {
  private apiUrl = `${environment.apiUrl}hr/Penalty`;
  constructor(
    private http: HttpClient,
    private querySupporter: QuerySupporterService
  ) { }

  getAllPenalties(filter: PenaltyFilter): Observable<PagedResult<PenaltyOverviewDto>> {
    const queryParams = this.querySupporter.getPenaltiesQueryParams(filter);
    return this.http.get<PagedResult<PenaltyOverviewDto>>(`${this.apiUrl}?`, {params: queryParams});
  }

  // 2. جلب تفاصيل عقوبة واحدة بالـ ID
  getPenaltyById(id: string): Observable<PenaltyDetailsDto> {
    return this.http.get<PenaltyDetailsDto>(`${this.apiUrl}/${id}`);
  }

  // 3. إضافة عقوبة جديدة (تتطلب FormData بسبب الملف)
  createPenalty(request: PenaltyCreateRequestDto): Observable<Result> {
    const formData = new FormData();
    formData.append('employeeId', request.employeeId);
    formData.append('amount', request.amount.toString());
    formData.append('reason', request.reason);
    formData.append('decisionFile', request.decisionFile, request.decisionFile.name);

    return this.http.post<Result>(this.apiUrl, formData);
  }

  // 4. تعديل عقوبة (تتطلب FormData بسبب الملف الاختياري)
  updatePenalty(id: string, request: PenaltyUpdateRequestDto): Observable<Result> {
    const formData = new FormData();
    formData.append('amount', request.amount.toString());
    formData.append('reason', request.reason);
    if (request.decision) {
      formData.append('decision', request.decision, request.decision.name);
    }

    return this.http.put<Result>(`${this.apiUrl}/${id}`, formData);
  }

  // 5. الموافقة/الرفض على عقوبة
  approvePenalty(id: string, isApproved: boolean): Observable<Result> {
    return this.http.put<Result>(`${this.apiUrl}/approve/${id}?isAproved=${isApproved}`, {});
  }

  // 6. حذف عقوبة
  deletePenalty(id: string): Observable<Result> {
    return this.http.delete<Result>(`${this.apiUrl}/${id}`);
  }

  getProxyPdfUrl(fileUrl: string): string {
    return `${environment.apiUrl}file/view-pdf?url=${fileUrl}`;
  }
}
