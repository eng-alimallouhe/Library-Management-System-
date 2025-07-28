import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink, Params } from '@angular/router';
import { debounceTime, finalize, tap } from 'rxjs/operators';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable, Subscription } from 'rxjs';

import { MatSliderModule } from '@angular/material/slider';

import { DepartmentService } from '../../../../services/hr/department.service';
import { DepartmentFilter } from '../../../../shared/filters/hr/department.filter';
import { DepartmentOverviewDto } from '../../../../shared/models/hr/departments/department-overview.dto';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { QuerySupporterService } from '../../../../shared/services/query-supporter.service';
import { SearchCommunicationService } from '../../../../shared/services/search-communication.service';
import { AddDepartmentComponent } from '../add-department/add-department.component';


@Component({
  selector: 'app-departments',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    RouterLink,
    AsyncPipe,
    LoaderComponent,
    MatSliderModule,
    AddDepartmentComponent, 
  ],
  templateUrl: './departments.component.html',
  styleUrls: ['./departments.component.css']
})
export class DepartmentsComponent implements OnInit, OnDestroy {

  departmentsPage$!: Observable<PagedResult<DepartmentOverviewDto>>;
  departmentsPageSnapshot!: PagedResult<DepartmentOverviewDto>;
  isLoadingDepartments = true;

  filterForm!: FormGroup;
  departmentFilter!: DepartmentFilter;

  showFilters = false;
  isSmallScreen = false;
  isDateRangeExpanded = true;
  isOnlyActiveExpanded = true;
  isEmployeeNumberExpanded = true;

  isDepartmentAddFormVisible = false; 

  minEmployeesRange = 0;
  maxEmployeesRange: number = 50; 

  private filterFormSubscription: Subscription | undefined;
  private searchNameSubscription: Subscription | undefined;
  private isInitializingForm: boolean = true; 

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  constructor(
    private fb: FormBuilder,
    private departmentService: DepartmentService,
    private router: Router,
    private route: ActivatedRoute,
    private querySupporter: QuerySupporterService,
    private translate: TranslateService,
    private searchService: SearchCommunicationService,
  ) { }

  ngOnInit(): void {
    this.initializeForm();
    this.checkScreenSize();
    
    this.departmentService.getMaxEmployeeCount()
      .pipe(
        finalize(() => {
          this.filterForm.patchValue({
            maxEmployeesNumber: this.maxEmployeesRange 
          }, { emitEvent: false });

          this.restoreFilterFromSessionOrURL(); 
          this.patchFormFromFilter();           
          
          this.listenToSearchName(); 

          this.isInitializingForm = false; 
          this.listenToFilterFormChanges(); 
          this.loadDepartments();           
        })
      )
      .subscribe({
        next: response => {
          this.maxEmployeesRange = response + 1; 
        },
        error: err => {
          console.error('فشل في جلب أقصى عدد للموظفين، استخدام قيمة افتراضية:', err);
          this.maxEmployeesRange = 1000; 
        }
      });
  }

  private checkScreenSize(): void {
    this.isSmallScreen = window.innerWidth <= 991;
    if (!this.isSmallScreen) {
      this.showFilters = true;
    }
  }

  private initializeForm() {
    this.filterForm = this.fb.group({
      from: [null],
      to: [null],
      onlyActiveRegisters: [true],
      minEmployeeNumber: [this.minEmployeesRange], 
      maxEmployeesNumber: [this.maxEmployeesRange]  
    });
  }

  private listenToFilterFormChanges() {
    if (this.isInitializingForm) {
      return; 
    }

    if (this.filterFormSubscription) {
      this.filterFormSubscription.unsubscribe();
    }
    this.filterFormSubscription = this.filterForm.valueChanges
      .pipe(
        debounceTime(500)
      )
      .subscribe(() => {
        if (!this.isInitializingForm) {
          this.updateDepartmentFilterFromForm();
          this.loadDepartments();
        }
      });
  }

  private updateDepartmentFilterFromForm() {
    const formValues = this.filterForm.value;
    
    const minEmployees = formValues.minEmployeeNumber; 
    const maxEmployees = formValues.maxEmployeesNumber; 

    this.departmentFilter = {
      ...this.departmentFilter, 
      from: formValues.from || null,
      to: formValues.to || null,
      onlyActiveRegisters: formValues.onlyActiveRegisters,
      minEmployeeCount: minEmployees, 
      maxEmployeeCount: maxEmployees, 
      pageNumber: 1
    };
  }

  private listenToSearchName() {
    if (this.searchNameSubscription) {
      this.searchNameSubscription.unsubscribe();
    }
    this.searchNameSubscription = this.searchService.nameSearch$.subscribe(name => {
      this.departmentFilter.name = name ? name : null;
      this.departmentFilter.pageNumber = 1; 
      this.loadDepartments(); 
    });
  }

  private patchFormFromFilter() {
    this.filterForm.patchValue({
      from: this.departmentFilter.from ? this.formatDate(this.departmentFilter.from) : null,
      to: this.departmentFilter.to ? this.formatDate(this.departmentFilter.to) : null,
      onlyActiveRegisters: this.departmentFilter.onlyActiveRegisters,
      minEmployeeNumber: this.departmentFilter.minEmployeeCount ?? this.minEmployeesRange,
      maxEmployeesNumber: this.departmentFilter.maxEmployeeCount ?? this.maxEmployeesRange 
    }, { emitEvent: false }); 
  }

  private formatDate(dateInput: Date | string): string {
    const date = new Date(dateInput);
    return date.toISOString().split('T')[0];
  }

  private restoreFilterFromSessionOrURL() {
    const storedFilter = sessionStorage.getItem('departmentFilter');
    const urlParams = this.route.snapshot.queryParamMap;

    if (urlParams.keys.length > 0) { 
      this.departmentFilter = {
        name: urlParams.get('name') || null,
        from: urlParams.get('from') || null,
        to: urlParams.get('to') || null,
        onlyActiveRegisters: urlParams.get('onlyActiveRegisters') === 'true',
        pageNumber: +(urlParams.get('pageNumber') || 1),
        pageSize: +(urlParams.get('pageSize') || 10),
        minEmployeeCount: urlParams.get('minEmployeeNumber') ? +urlParams.get('minEmployeeNumber')! : null,
        maxEmployeeCount: urlParams.get('maxEmployeesNumber') ? +urlParams.get('maxEmployeesNumber')! : null,
        language: 0
      };
    } else if (storedFilter) {
      this.departmentFilter = JSON.parse(storedFilter);
      sessionStorage.removeItem('departmentFilter'); 

      const queryParams = this.querySupporter.getDepartmentsQueryParams(this.departmentFilter);
      this.router.navigate([], { queryParams, replaceUrl: true }); 
    } else {
      this.departmentFilter = {
        name: null,
        from: null,
        to: null,
        onlyActiveRegisters: true,
        pageNumber: 1,
        pageSize: 10,
        minEmployeeCount: null,
        maxEmployeeCount: null,
        language: 0
      };
      const queryParams = this.querySupporter.getDepartmentsQueryParams(this.departmentFilter);
      this.router.navigate([], { queryParams, replaceUrl: true }); 
    }
  }

  private loadDepartments() {
    const queryParams = this.querySupporter.getDepartmentsQueryParams(this.departmentFilter);

    this.router.navigate([], { queryParams: queryParams, replaceUrl: true }); 

    this.isLoadingDepartments = true;
    
    this.departmentsPage$ = this.departmentService
      .getAllDepartments(this.departmentFilter)
      .pipe(
        finalize(() => {
          this.isLoadingDepartments = false;
        }),
        tap(page => this.departmentsPageSnapshot = page)
      );
  }

  applyFilters() {
    this.updateDepartmentFilterFromForm();
    this.departmentFilter.pageNumber = 1; 
    this.loadDepartments();
  }

  clearAllFilters() {
    this.isInitializingForm = true; 
    this.filterForm.reset({
      from: null,
      to: null,
      onlyActiveRegisters: true,
      minEmployeeNumber: this.minEmployeesRange, 
      maxEmployeesNumber: this.maxEmployeesRange 
    }, { emitEvent: false }); 
    this.departmentFilter.name = null; 
    this.updateDepartmentFilterFromForm(); 
    this.departmentFilter.pageNumber = 1; 
    this.isInitializingForm = false; 
    this.loadDepartments(); 
  }

  clearDateRangeFilter() {
    this.isInitializingForm = true; 
    this.filterForm.patchValue({
      from: null,
      to: null
    }, { emitEvent: false }); 
    this.updateDepartmentFilterFromForm(); 
    this.isInitializingForm = false; 
    this.loadDepartments(); 
  }

  clearEmployeeNumberFilter() {
    this.isInitializingForm = true; 
    this.filterForm.patchValue({
      minEmployeeNumber: this.minEmployeesRange, 
      maxEmployeesNumber: this.maxEmployeesRange 
    }, { emitEvent: false }); 
    this.updateDepartmentFilterFromForm(); 
    this.isInitializingForm = false; 
    this.loadDepartments(); 
  }

  clearOnlyActiveRegistersFilter() {
      this.isInitializingForm = true; 
      this.filterForm.patchValue({
        onlyActiveRegisters: true
      }, { emitEvent: false }); 
      this.updateDepartmentFilterFromForm(); 
      this.isInitializingForm = false; 
      this.loadDepartments(); 
  }

  getNextPage() {
    if (this.departmentFilter.pageNumber! < this.departmentsPageSnapshot.totalPages) {
      this.departmentFilter.pageNumber!++;
      this.loadDepartments();
    }
  }

  getPreviousPage() {
    if (this.departmentFilter.pageNumber && this.departmentFilter.pageNumber > 1) {
      this.departmentFilter.pageNumber--;
      this.loadDepartments();
    }
  }

  goToPage(page: number) {
    if (page !== this.departmentFilter.pageNumber) {
      this.departmentFilter.pageNumber = page;
      this.loadDepartments();
    }
  }

  getPageNumbers(currentPage: number, totalPages: number, maxPagesToShow: number = 5): number[] {
    const pages: number[] = [];
    const half = Math.floor(maxPagesToShow / 2);
    let start = Math.max(1, currentPage - half);
    let end = Math.min(totalPages, currentPage + half);

    if (end - start + 1 < maxPagesToShow) {
      if (start === 1) {
        end = Math.min(totalPages, start + maxPagesToShow - 1);
      } else if (end === totalPages) {
        start = Math.max(1, end - maxPagesToShow + 1);
      }
    }

    for (let i = start; i <= end; i++) {
      pages.push(i);
    }
    return pages;
  }

  toggleFilters() {
    this.showFilters = !this.showFilters;
  }

  toggleDateRange() {
    this.isDateRangeExpanded = !this.isDateRangeExpanded;
  }

  toggleOnlyActive() {
    this.isOnlyActiveExpanded = !this.isOnlyActiveExpanded;
  }

  toggleEmployeeNumber() {
    this.isEmployeeNumberExpanded = !this.isEmployeeNumberExpanded;
  }

  get currentMinEmployeeCount(): number {
    return this.filterForm.get('minEmployeeNumber')?.value ?? this.minEmployeesRange;
  }

  get currentMaxEmployeeCount(): number {
    return this.filterForm.get('maxEmployeesNumber')?.value ?? this.maxEmployeesRange;
  }

  showDepartmentAddForm() {
    this.isDepartmentAddFormVisible = true;
  }

  onDepartmentAdded(): void {
    this.loadDepartments();
  }

  navigateToDetails(departmentId: string) {
    sessionStorage.setItem('departmentFilter', JSON.stringify(this.departmentFilter));
    this.router.navigate([`/app/departments/${departmentId}`]);
  }

  ngOnDestroy(): void {
    sessionStorage.setItem('departmentFilter', JSON.stringify(this.departmentFilter));
    this.filterFormSubscription?.unsubscribe();
    this.searchNameSubscription?.unsubscribe();
  }
}
