import { Component, OnDestroy, OnInit } from '@angular/core';
import { AsyncPipe, CommonModule } from '@angular/common';
import { DashboardKPIDto } from '../../../shared/models/admin/dashboard-kpi.dto';
import { finalize, map, Observable, pipe } from 'rxjs';
import { DashboardService } from '../../../services/admin/dashboard.service';
import { TranslatePipe } from '@ngx-translate/core';
import { RouterLink } from '@angular/router';
import { LoaderComponent } from "../../../shared/components/loader/loader.component";
import { SalesChartComponent } from "../sales-chart/sales-chart.component";
import { OrdersChartComponent } from "../orders-chart/orders-chart.component";
import { ProductFilter } from '../../../shared/filters/library-management/products.filter';
import { StockSnapshotDto } from '../../../shared/models/library-management/stock-snapshot.dto';
import { PagedResult } from '../../../shared/models/results/paged-result.dto';

interface KpiCardData {
  icon: string;
  title: string;
  value: number;
  description: string;
  trendValue: number;
  lastUpdate: string;
  routerLink?: string | any[];
}


@Component({
  selector: 'app-dashboard',
  imports: [
    TranslatePipe,
    CommonModule,
    AsyncPipe,
    RouterLink,
    LoaderComponent,
    SalesChartComponent,
    OrdersChartComponent
],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent implements OnInit, OnDestroy{
  kpiCards$!: Observable<KpiCardData[]>;
  isLoadingKpi: boolean = false;
  lowStock$!: Observable<PagedResult<StockSnapshotDto>>;
  isLoadingProducts = true;

  constructor(
    private dashboardService: DashboardService) {
  }

  ngOnDestroy(): void {
    console.log('deasborad destroyed');
  }
    
  ngOnInit(): void {
    let productFilter: ProductFilter = {
      language: 0,
      maxQuantity: 50,
      minQuantity: 0,
      pageNumber: 1,
      pageSize: 15,
      onlyActiveRegisters: true
    };
    

     this.lowStock$ = this.dashboardService.getLowStock(productFilter)
     .pipe(finalize(() => {
      this.isLoadingProducts = false;
     }));

    const currentDate = this.getMonthBeforeNow();
    const dateToSend = currentDate.toISOString();
    
    this.isLoadingKpi = true;
    this.kpiCards$ = this.dashboardService.getDashboardKpi(dateToSend).pipe(
      map(data => this.mapKpiDataToCards(data)),
      finalize(() => this.isLoadingKpi = false)
    );
  }

  getMonthBeforeNow(): Date {
  const today = new Date();
  let targetMonth = today.getMonth() - 1;
  let targetYear = today.getFullYear();

  if (targetMonth < 0) {
    targetMonth += 12;
    targetYear--;
  }

  return new Date(targetYear, targetMonth, 1);
  }

  private mapKpiDataToCards(data: DashboardKPIDto): KpiCardData[] {
    const cards: KpiCardData[] = [];
    const comparisonTextKey = 'KPI.COMPARISON_PREVIOUS_MONTH';

    cards.push({
      icon: 'people',
      title: 'KPI.TOTAL_USERS_TITLE',
      value: data.usersCount,
      description: 'KPI.TOTAL_USERS_DESCRIPTION',
      trendValue: data.usersChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/users'
    });
    cards.push({
      icon: 'work',
      title: 'KPI.EMPLOYEES_COUNT_TITLE',
      value: data.employeesCount,
      description: 'KPI.EMPLOYEES_DESCRIPTION',
      trendValue: data.employeesChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/employees'
    });
    cards.push({
      icon: 'sentiment_very_satisfied',
      title: 'KPI.TOTAL_CUSTOMERS_TITLE',
      value: data.customersCount,
      description: 'KPI.ACTIVE_CUSTOMERS_DESCRIPTION',
      trendValue: data.customersChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/customers'
    });
    cards.push({
      icon: 'person_add',
      title: 'KPI.NEW_CUSTOMERS_TITLE',
      value: data.newCustomersCount,
      description: 'KPI.NEW_CUSTOMERS_DESCRIPTION',
      trendValue: data.customersChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/customers/new'
    });
    cards.push({
      icon: 'pending_actions',
      title: 'KPI.TOTAL_ORDERS_TITLE',
      value: data.ordersCount,
      description: 'KPI.TOTAL_ORDERS_DESCRIPTION',
      trendValue: data.ordersChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/orders'
    });
    cards.push({
      icon: 'book',
      title: 'KPI.TOTAL_BOOKS_TITLE',
      value: data.booksCount,
      description: 'KPI.BOOKS_AVAILABLE_DESCRIPTION',
      trendValue: data.booksChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/books'
    });
    cards.push({
      icon: 'library_add',
      title: 'KPI.NEW_BOOKS_TITLE',
      value: data.newBooksCount,
      description: 'KPI.NEW_BOOKS_DESCRIPTION',
      trendValue: data.booksChangePercentage,
      lastUpdate: comparisonTextKey,
      routerLink: '/admin/books/new'
    });

    return cards;
  }

}
