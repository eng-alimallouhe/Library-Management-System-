import { Component, OnDestroy, OnInit } from '@angular/core';

import {
  ChartComponent,
  ApexAxisChartSeries,
  ApexXAxis,
  ApexDataLabels,
  ApexTooltip,
  ApexStroke,
  ApexTitleSubtitle,
  ApexLegend,
  ApexYAxis,
  ApexFill,
  ApexPlotOptions,
  ApexGrid,
  NgApexchartsModule
} from 'ng-apexcharts';
import { DashboardService } from '../../../services/admin/dashboard.service';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { ThemeService } from '../../../shared/services/theme.service';
import { map, catchError, of, finalize, Subject, takeUntil, Subscription } from 'rxjs';
import { MonthlySalesDto } from '../../../shared/models/admin/monthly-sales.dto ';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from "../../../shared/components/loader/loader.component";



export type ChartOptions = {
  series: ApexAxisChartSeries;
  chart: any;
  xaxis: ApexXAxis;
  dataLabels: ApexDataLabels;
  yaxis: ApexYAxis;
  stroke: ApexStroke;
  title: ApexTitleSubtitle;
  tooltip: ApexTooltip;
  legend: ApexLegend;
  fill: ApexFill;
  plotOptions?: ApexPlotOptions;
  grid?: ApexGrid;
};





@Component({
  selector: 'app-sales-chart',
  imports: [
    NgApexchartsModule,
    CommonModule,
    TranslatePipe,
    LoaderComponent
],
  templateUrl: './sales-chart.component.html',
  styleUrl: './sales-chart.component.css'
})
export class SalesChartComponent implements OnInit, OnDestroy{

  salesChartOptions!: Partial<ChartOptions>;
  isLoadingSalesChart: boolean = true;

  private themeSubscription!: Subscription;

  constructor(
    private dashboardService: DashboardService,
    private translate: TranslateService,
    private themeService: ThemeService
  ){}
  

  ngOnInit(){
    this.fetchMonthlySalesData();


    this.themeSubscription = this.themeService.currentTheme$.subscribe(theme => {
      this.updateSalesChartColors(theme);
    });
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

  private fetchMonthlySalesData(): void {
    const today = new Date();
    const startDate = new Date(today.getFullYear(), today.getMonth() - 4, 1);
    const endDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    const startDateISO = startDate.toISOString().split('T')[0];
    const endDateISO = endDate.toISOString().split('T')[0];

    this.dashboardService.getMonthlySales(startDateISO, endDateISO).pipe(
      map(data => this.processSalesChartData(data, startDate, endDate)),
      catchError(error => {
        console.error('[DashboardComponent] Error fetching monthly sales data:', error);
        return of({});
      }),
      finalize(() => {
        this.isLoadingSalesChart = false;
      })
    ).subscribe(chartOptions => {
      this.salesChartOptions = chartOptions;
      this.updateSalesChartColors(this.themeService.getCurrentTheme());
    });
  }

  ngOnDestroy(){
    this.themeSubscription.unsubscribe();
  }

  private processSalesChartData(
    salesData: MonthlySalesDto[],
    startDate: Date,
    endDate: Date
  ): Partial<ChartOptions> {
    const xaxisTitle = this.translate.instant('CHARTS.SALES_CHART.SALES_CHART_XAXIS_TITLE');
    const yaxisTitle = this.translate.instant('CHARTS.SALES_CHART.SALES_CHART_YAXIS_TITLE');
    const columnTitle = this.translate.instant('CHARTS.SALES_CHART.SALES_CHART_COLUMN_TITLE');

    const salesMap = new Map<string, number>();
    salesData.forEach(item => {
      const key = `${item.year}-${item.month}`;
      salesMap.set(key, item.totalSales);
    });

    const categories: string[] = [];
    const totalSalesValues: number[] = [];

    let currentMonth = new Date(startDate.getFullYear(), startDate.getMonth(), 1);
    const endMonth = new Date(endDate.getFullYear(), endDate.getMonth(), 1);

    while (currentMonth <= endMonth) {
      const year = currentMonth.getFullYear();
      const month = currentMonth.getMonth() + 1;
      const key = `${year}-${month}`;
      const salesValue = salesMap.get(key) || 0;      

      let monthName = this.translate.instant(`MONTHS.${month}`);

      categories.push(`${monthName}-${year}`);
      totalSalesValues.push(salesValue);

      currentMonth.setMonth(currentMonth.getMonth() + 1);
    }

    return {
      series: [
        {
          name: columnTitle,
          data: totalSalesValues
        }
      ],
      chart: {
        type: 'bar',
        height: 350,
        width: '100%',
        toolbar: { show: true },
        fontFamily: 'Open Sans, sans-serif',
        background: '#fff'
      },
      plotOptions: {
        bar: {
          horizontal: false,
          columnWidth: '55%'
        }
      },
      xaxis: {
        categories,
        title: {
          text: xaxisTitle,
          style: {
            fontSize: '17px',
            fontFamily: 'Open Sans, sans-serif',
            fontWeight: 'light',
            color: '#333'
          }
        },
        labels: {
          style: {
            fontSize: '14px',
            fontFamily: 'Open Sans, sans-serif',
            fontWeight: 'light',
            colors: '#333'
          }
        }
      },
      yaxis: {
        title: {
          text: yaxisTitle,
          style: {
            fontSize: '17px',
            fontFamily: 'Open Sans, sans-serif',
            fontWeight: 'light',
            color: '#333' 
          }
        },
        labels: {
          style: {
            fontFamily: 'Open Sans, sans-serif',
            fontWeight: 'light',
            colors: '#333' 
          }
        }
      },
      dataLabels: {
        enabled: false
      },
      stroke: {
        show: true,
        width: 2,
        colors: ['transparent']
      },
      fill: {
        opacity: 1,
        colors: ['#00a180'] 
      },
      tooltip: {
        y: {
          formatter: (val: number) => `${val.toFixed(2)} $`
        }
      },
      legend: {
        show: true,
        fontFamily: 'Roboto, sans-serif',
        labels: {
          colors: ['#333'] 
        }
      },
      title: {
        text: '',
        align: 'center',
        style: {
          fontSize: '20px',
          fontWeight: 'bold',
          fontFamily: 'Roboto, sans-serif',
          color: '#333'
        }
      },
      grid: {
        strokeDashArray: 4,
        borderColor: '#eee' 
      }
    };

  }


  private updateSalesChartColors(theme: 'light_mode' | 'dark_mode') {
    const isDark = theme === 'dark_mode';

    
    if (!this.salesChartOptions) return;

    
    const darkColors = {
      textColor: '#ccc',
      gridColor: '#d5d5d5',
      barColor: '#00a180',
      backgroundColor: '#2b2c2f',
      tooltipTheme: 'dark'
    };

    const lightColors = {
      textColor: '#333',
      gridColor: '#eee',
      barColor: '#00a180',
      backgroundColor: '#fff',
      tooltipTheme: 'light'
    };

    const colors = isDark ? darkColors : lightColors;

    this.salesChartOptions = {
      ...this.salesChartOptions,
      chart: {
        ...this.salesChartOptions.chart,
        background: colors.backgroundColor,
        fontFamily: 'IBM Plex Sans Arabic, sans-serif'
      },
      xaxis: {
        ...this.salesChartOptions.xaxis,
        labels: {
          ...this.salesChartOptions.xaxis?.labels,
          style: {
            ...this.salesChartOptions.xaxis?.labels?.style,
            colors: colors.textColor
          }
        },
        title: {
          ...this.salesChartOptions.xaxis?.title,
          style: {
            ...this.salesChartOptions.xaxis?.title?.style,
            color: colors.textColor
          }
        }
      },
      yaxis: {
        ...this.salesChartOptions.yaxis,
        labels: {
          ...this.salesChartOptions.yaxis?.labels,
          style: {
            ...this.salesChartOptions.yaxis?.labels?.style,
            colors: colors.textColor
          }
        },
        title: {
          ...this.salesChartOptions.yaxis?.title,
          style: {
            ...this.salesChartOptions.yaxis?.title?.style,
            color: colors.textColor
          }
        }
      },
      fill: {
        ...this.salesChartOptions.fill,
        colors: [colors.barColor]
      },
      grid: {
        ...this.salesChartOptions.grid,
        borderColor: colors.gridColor
      },
      legend: {
        ...this.salesChartOptions.legend,
        labels: {
          colors: [colors.textColor]
        }
      },
      title: {
        ...this.salesChartOptions.title,
        style: {
          ...this.salesChartOptions.title?.style,
          color: colors.textColor
        }
      },
      tooltip: {
        ...this.salesChartOptions.tooltip,
        theme: colors.tooltipTheme
      }
    };
  }

}
