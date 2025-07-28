import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { LanguageService } from '../../shared/services/language.service';
import { DashboardKPIDto } from '../../shared/models/admin/dashboard-kpi.dto';
import { MonthlyOrdersDto } from '../../shared/models/admin/monthly-orders.dto';
import { MonthlySalesDto } from '../../shared/models/admin/monthly-sales.dto ';
import { TopSellingProductDto } from '../../shared/models/admin/top-sales-products.dto';
import { QuerySupporterService } from '../../shared/services/query-supporter.service';
import { ProductFilter } from '../../shared/filters/library-management/products.filter';
import { StockSnapshotDto } from '../../shared/models/library-management/stock-snapshot.dto';
import { PagedResult } from '../../shared/models/results/paged-result.dto';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  apiUrl: string = environment.apiUrl + `admin/Dashboard`;
  
  constructor(
    private http: HttpClient,
    private languageService: LanguageService,
    private querySupporter: QuerySupporterService
  ) { }


  getDashboardKpi(date: string) : Observable<DashboardKPIDto> {
    return this.http.get<DashboardKPIDto>(`${this.apiUrl}/Kpi/${date}`);
  }

  getMonthlySales(startDate: string, endDate: string): Observable<MonthlySalesDto[]> {
    return this.http.get<MonthlySalesDto[]>(`${this.apiUrl}/monthly-sales?From=${startDate}&To=${endDate}`);
  }

  getMonthlyOrders(startDate: string, endDate: string): Observable<MonthlyOrdersDto[]> {
    return this.http.get<MonthlyOrdersDto[]>(`${this.apiUrl}/monthly-orders?From=${startDate}&To=${endDate}`);
  }

  getTopSellingProducts() : Observable<TopSellingProductDto[]> {
    const language = this.languageService.getLanguageAsNumber();
    
    return this.http.get<TopSellingProductDto[]>(`${this.apiUrl}/top-sales-products?language=${language}`);
  }

  getLowStock(filter: ProductFilter) : Observable<PagedResult<StockSnapshotDto>> {
    const queryParams = this.querySupporter.getProductsQueryParameters(filter);
    return this.http.get<PagedResult<StockSnapshotDto>>(`${this.apiUrl}/low-stock`, { params: queryParams });
  }
}