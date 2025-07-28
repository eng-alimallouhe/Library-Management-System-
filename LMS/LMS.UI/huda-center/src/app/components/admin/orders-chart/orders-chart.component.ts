import { CommonModule, AsyncPipe } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { DashboardService } from '../../../services/admin/dashboard.service';
import { StorageService } from '../../../shared/services/storage.service';
import { ThemeService } from '../../../shared/services/theme.service';
import { catchError, finalize, map, of, Subscription } from 'rxjs';
import { MonthlyOrdersDto } from '../../../shared/models/admin/monthly-orders.dto';


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
  selector: 'app-orders-chart',
  imports: [
    TranslatePipe,
    CommonModule,
    NgApexchartsModule,
    LoaderComponent
],
  templateUrl: './orders-chart.component.html',
  styleUrl: './orders-chart.component.css'
})
export class OrdersChartComponent {
  ordersChartOptions!: Partial<ChartOptions>;
  isLoadingOrdersChart: boolean = true;
  

  private themeSubscription!: Subscription;

  constructor(
    private dashboardService: DashboardService,
      private storageService: StorageService,
      private translate: TranslateService,
      private themeService: ThemeService
  ) {}


  ngOnInit(): void {
    this.isLoadingOrdersChart = true;
    this.fetchMonthlyOrdersData();

    this.themeSubscription = this.themeService.currentTheme$.subscribe(theme => {
      this.updateOrdersChartColors(theme);
    });
  }


  
  ngOnDestroy(){
    this.themeSubscription.unsubscribe();
  }


  private fetchMonthlyOrdersData(): void {
    const today = new Date();
    const startDate = new Date(today.getFullYear(), today.getMonth() - 4, 1);
    const endDate = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    const startDateISO = startDate.toISOString().split('T')[0];
    const endDateISO = endDate.toISOString().split('T')[0];

    this.dashboardService.getMonthlyOrders(startDateISO, endDateISO).pipe(
      map(data => this.processOrdersChartData(data, startDate, endDate)),
      catchError(error => {
        console.error('[DashboardComponent] Error fetching monthly sales data:', error);
        return of({}); 
      }),
      finalize(() => {
        this.isLoadingOrdersChart = false;
      })
    ).subscribe(chartOptions => {
      this.ordersChartOptions = chartOptions;
      
      this.updateOrdersChartColors(this.themeService.getCurrentTheme());
    });
  }


  private processOrdersChartData(
    ordersData: MonthlyOrdersDto[],
    startDate: Date,
    endDate: Date
  ): Partial<ChartOptions> {
    const xaxisTitle = this.translate.instant('CHARTS.ORDERS_CHART.ORDERS_CHART_XAXIS_TITLE');
    const yaxisTitle = this.translate.instant('CHARTS.ORDERS_CHART.ORDERS_CHART_YAXIS_TITLE');
    const columnTitle = this.translate.instant('CHARTS.ORDERS_CHART.ORDERS_CHART_COLUMN_TITLE');

    const ordersMap = new Map<string, number>();
    ordersData.forEach(item => {
      const key = `${item.year}-${item.month}`;
      ordersMap.set(key, item.totalOrdersCount);
    });

    const categories: string[] = [];
    const totalOrdersValues: number[] = [];

    let currentMonth = new Date(startDate.getFullYear(), startDate.getMonth(), 1);
    const endMonth = new Date(endDate.getFullYear(), endDate.getMonth(), 1);

    while (currentMonth <= endMonth) {
      const year = currentMonth.getFullYear();
      const month = currentMonth.getMonth() + 1;
      const key = `${year}-${month}`;
      const ordersValue = ordersMap.get(key) || 0;

      const monthName = this.translate.instant(`MONTHS.${month}`);
      categories.push(`${monthName} ${year}`);
      totalOrdersValues.push(ordersValue);

      currentMonth.setMonth(currentMonth.getMonth() + 1);
    }

    return {
      series: [
        {
          name: columnTitle,
          data: totalOrdersValues
        }
      ],
      chart: {
        type: 'bar',
        height: 350,
        toolbar: { show: true },
        fontFamily: 'Roboto, sans-serif',
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
            fontFamily: '"Open Sans", sans-serif',
            fontWeight: 'light',
            color: '#333'
          }
        },
        labels: {
          style: {
            fontSize: '14px',
            fontFamily: '"Open Sans", sans-serif',
            fontWeight: 'light',
            colors: '#333'
          }
        }
      },
      yaxis: {
        title: {
          text: yaxisTitle,
          style: {
            fontSize: '16px',
            fontWeight: 'light',
            fontFamily: 'Open Sans, sans-serif',
            color: '#333' 
          }
        },
        labels: {
          style: {
            fontFamily: '"Open Sans", sans-serif',
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
          formatter: (val: number) => `${val.toFixed(0)}`
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
        text: ' ',
        align: 'center',
        style: {
          fontSize: '20px',
          fontWeight: 'bold',
          fontFamily: 'Open Sans, sans-serif',
          color: '#333'
        }
      },
      grid: {
        strokeDashArray: 4,
        borderColor: '#eee' 
      }
    };
  }


  private updateOrdersChartColors(theme: 'light_mode' | 'dark_mode') {  
    const isDark = theme === 'dark_mode';

    
    if (!this.ordersChartOptions) return;

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

    this.ordersChartOptions = {
      ...this.ordersChartOptions,
      chart: {
        ...this.ordersChartOptions.chart,
        background: colors.backgroundColor,
        fontFamily: 'IBM Plex Sans Arabic, sans-serif'
      },
      xaxis: {
        ...this.ordersChartOptions.xaxis,
        labels: {
          ...this.ordersChartOptions.xaxis?.labels,
          style: {
            ...this.ordersChartOptions.xaxis?.labels?.style,
            colors: colors.textColor
          }
        },
        title: {
          ...this.ordersChartOptions.xaxis?.title,
          style: {
            ...this.ordersChartOptions.xaxis?.title?.style,
            color: colors.textColor
          }
        }
      },
      yaxis: {
        ...this.ordersChartOptions.yaxis,
        labels: {
          ...this.ordersChartOptions.yaxis?.labels,
          style: {
            ...this.ordersChartOptions.yaxis?.labels?.style,
            colors: colors.textColor
          }
        },
        title: {
          ...this.ordersChartOptions.yaxis?.title,
          style: {
            ...this.ordersChartOptions.yaxis?.title?.style,
            color: colors.textColor
          }
        }
      },
      fill: {
        ...this.ordersChartOptions.fill,
        colors: [colors.barColor]
      },
      grid: {
        ...this.ordersChartOptions.grid,
        borderColor: colors.gridColor
      },
      legend: {
        ...this.ordersChartOptions.legend,
        labels: {
          colors: [colors.textColor]
        }
      },
      title: {
        ...this.ordersChartOptions.title,
        style: {
          ...this.ordersChartOptions.title?.style,
          color: colors.textColor
        }
      },
      tooltip: {
        ...this.ordersChartOptions.tooltip,
        theme: colors.tooltipTheme
    }
    };
  }

}
