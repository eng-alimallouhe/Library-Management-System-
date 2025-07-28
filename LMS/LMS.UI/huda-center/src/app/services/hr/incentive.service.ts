import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { PagedResult } from '../../shared/models/results/paged-result.dto'; 

import { IncentiveOverviewDto } from '../../shared/models/hr/incentives/incentive-overview.dto';
import { IncentiveDetailsDto } from '../../shared/models/hr/incentives/incentive-details.dto';
import { IncentiveCreateRequestDto } from '../../shared/models/hr/incentives/incentive-create-request.dto';
import { IncentiveUpdateRequestDto } from '../../shared/models/hr/incentives/incentive-update-request.dto';
import { Result } from '../../shared/models/result/result';
import { IncentiveFilter } from '../../shared/filters/hr/incentive.filter';
import { QuerySupporterService } from '../../shared/services/query-supporter.service';


@Injectable({
  providedIn: 'root'
})
export class IncentiveService {
  private apiUrl = `${environment.apiUrl}hr/Incentive`;

  constructor(
    private http: HttpClient,
    private queryHelper: QuerySupporterService
) { }

  getAllIncentives(filter: IncentiveFilter): Observable<PagedResult<IncentiveOverviewDto>> {
    const queryParams = this.queryHelper.getIncentivesQueryParams(filter);
     return this.http.get<PagedResult<IncentiveOverviewDto>>(`${this.apiUrl}`, { params:  queryParams });
  }

  getIncentiveById(id: string): Observable<IncentiveDetailsDto> {
    return this.http.get<IncentiveDetailsDto>(`${this.apiUrl}/${id}`);
  }

  createIncentive(request: IncentiveCreateRequestDto): Observable<Result> {
    const formData = new FormData();
    formData.append('employeeId', request.employeeId);
    formData.append('amount', request.amount.toString());
    formData.append('reason', request.reason);
    formData.append('decisionFile', request.decisionFile, request.decisionFile.name);

    return this.http.post<Result>(this.apiUrl, formData);
  }

  updateIncentive(id: string, request: IncentiveUpdateRequestDto): Observable<Result> {
    const formData = new FormData();
    formData.append('amount', request.amount.toString());
    formData.append('reason', request.reason);
    if (request.decision) {
      formData.append('Decision', request.decision, request.decision.name); 
    }

    return this.http.put<Result>(`${this.apiUrl}/${id}`, formData);
  }

  approveRejectIncentive(id: string, isApproved: boolean): Observable<Result> {
    return this.http.put<Result>(`${this.apiUrl}/approve/${id}?isAproved=${isApproved}`, {}); 
  }

  getProxyFileUrl(fileUrl: string): string {
    return `${environment.apiUrl}file/view-pdf?url=${fileUrl}`; 
  }
}
