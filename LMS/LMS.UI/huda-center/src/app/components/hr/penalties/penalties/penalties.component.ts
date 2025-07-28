// src/app/components/hr/penalties/penalties.component.ts

import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormArray, FormControl } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink, Params } from '@angular/router';
import { debounceTime, finalize, tap } from 'rxjs/operators';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable, Subscription } from 'rxjs';

// استيراد الخدمات والـ DTOs والفلاتر
import { PenaltyService } from '../../../../services/hr/penalty.service';
import { DepartmentService } from '../../../../services/hr/department.service';
import { PenaltyFilter, PenaltyOrderBy } from '../../../../shared/filters/hr/penalty-filter';
import { PenaltyOverviewDto } from '../../../../shared/models/hr/penalties/penalty-overview.dto';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { QuerySupporterService } from '../../../../shared/services/query-supporter.service';
import { SearchCommunicationService } from '../../../../shared/services/search-communication.service';
import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { AvaliableDepartmentDto } from '../../../../shared/models/hr/departments/avaliable-departments.dto';

import { AddPenaltyComponent } from '../add-penalty/add-penalty.component';


@Component({
  selector: 'app-penalties',
  standalone: true,
  imports: [
    TranslatePipe,
    AsyncPipe,
    CommonModule,
    ReactiveFormsModule,
    LoaderComponent,
    LocalDatePipe,
    RouterLink
  ],
  templateUrl: './penalties.component.html',
  styleUrls: ['./penalties.component.css']
})
export class PenaltiesComponent implements OnInit, OnDestroy {

  penaltiesPage$!: Observable<PagedResult<PenaltyOverviewDto>>;
  penaltiesPageSnapshot!: PagedResult<PenaltyOverviewDto>;
  isLoadingPenalties = true;

  filterForm!: FormGroup;
  penaltyFilter!: PenaltyFilter;

  showFilters = false; // للتحكم في رؤية الـ overlay
  isSmallScreen = false;

  isDateRangeExpanded = true;
  isOnlyActiveExpanded = true;
  isDepartmentsExpanded = true;
  isAppliedExpanded = true;
  isApprovedExpanded = true;
  isOrderExpanded = true;

  isAddPenaltyFormVisible = false;

  availableDepartments: AvaliableDepartmentDto[] = [];
  isLoadingDepartments = true;

  private filterFormSubscription: Subscription | undefined;
  private searchNameSubscription: Subscription | undefined;
  private isInitializingForm: boolean = true;

  penaltyOrderByOptions = [
    { value: PenaltyOrderBy.ByEmployeeName, label: 'HR.PENALTIES.ORDER_BY.BY_EMPLOYEE_NAME' },
    { value: PenaltyOrderBy.ByAmount, label: 'HR.PENALTIES.ORDER_BY.BY_AMOUNT' },
    { value: PenaltyOrderBy.ByReason, label: 'HR.PENALTIES.ORDER_BY.BY_REASON' },
    { value: PenaltyOrderBy.ByDate, label: 'HR.PENALTIES.ORDER_BY.BY_DATE' },
    { value: PenaltyOrderBy.ByIsApplied, label: 'HR.PENALTIES.ORDER_BY.BY_IS_APPLIED' },
    { value: PenaltyOrderBy.ByIsApproved, label: 'HR.PENALTIES.ORDER_BY.BY_IS_APPROVED' },
  ];

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  constructor(
    private fb: FormBuilder,
    private penaltyService: PenaltyService,
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

    this.departmentService.getAvaliableDepartments(null)
      .pipe(
        finalize(() => {
          this.isLoadingDepartments = false;
          this.restoreFilterFromSessionOrURL();
          this.patchFormFromFilter();
          this.listenToFilterFormChanges();
          this.listenToSearchName();
          this.isInitializingForm = false;
          this.loadPenalties();
        })
      )
      .subscribe({
        next: (deps) => {
          this.availableDepartments = deps;
        },
        error: (err) => {
          console.error('Error loading available departments:', err);
        }
      });
  }

  private checkScreenSize(): void {
    this.isSmallScreen = window.innerWidth <= 991;
    if (!this.isSmallScreen) {
      this.showFilters = true;
    } else {
      this.showFilters = false;
    }
  }

  private initializeForm() {
    this.filterForm = this.fb.group({
      from: [null],
      to: [null],
      onlyActiveRegisters: [true],
      departmentIds: this.fb.array([]),
      byIsApplied: [null],
      byIsApproved: [null],
      orderBy: [PenaltyOrderBy.ByDate],
      isDesc: [true]
    });
  }

  get departmentIdsFormArray() {
    return this.filterForm.get('departmentIds') as FormArray;
  }

  onDepartmentCheckboxChange(event: any): void {
    const departmentId = event.target.value;
    const isChecked = event.target.checked;

    if (isChecked) {
      this.departmentIdsFormArray.push(new FormControl(departmentId));
    } else {
      const index = this.departmentIdsFormArray.controls.findIndex(x => x.value === departmentId);
      if (index >= 0) {
        this.departmentIdsFormArray.removeAt(index);
      }
    }
  }

  isDepartmentSelected(departmentId: string): boolean {
    return this.departmentIdsFormArray.controls.some(control => control.value === departmentId);
  }


  private listenToFilterFormChanges() {
    if (this.filterFormSubscription) {
      this.filterFormSubscription.unsubscribe();
    }
    this.filterFormSubscription = this.filterForm.valueChanges
      .pipe(
        debounceTime(500)
      )
      .subscribe(() => {
        if (!this.isInitializingForm) {
          this.updatePenaltyFilterFromForm();
          this.loadPenalties();
        }
      });
  }

  private updatePenaltyFilterFromForm() {
    const formValues = this.filterForm.value;
    
    this.penaltyFilter = {
      ...this.penaltyFilter,
      from: formValues.from || null,
      to: formValues.to || null,
      onlyActiveRegisters: formValues.onlyActiveRegisters,
      language: 0,
      departmentIds: formValues.departmentIds,
      byIsApplied: formValues.byIsApplied,
      byIsApproved: formValues.byIsApproved,
      orderBy: formValues.orderBy,
      isDesc: formValues.isDesc,
      pageNumber: 1
    };
  }

  private listenToSearchName() {
    if (this.searchNameSubscription) {
      this.searchNameSubscription.unsubscribe();
    }
    this.searchNameSubscription = this.searchService.nameSearch$.subscribe(name => {
      this.penaltyFilter.name = name ? name : null;
      this.penaltyFilter.pageNumber = 1; 
      this.loadPenalties(); 
    });
  }

  private restoreFilterFromSessionOrURL() {
    const storedFilter = sessionStorage.getItem('penaltyFilter');
    const urlParams = this.route.snapshot.queryParamMap;

    if (urlParams.keys.length > 0) { 
      this.penaltyFilter = {
        pageNumber: +(urlParams.get('pageNumber') || 1),
        pageSize: +(urlParams.get('pageSize') || 10),
        name: urlParams.get('name') || null,
        language: +(urlParams.get('language') || 0),
        from: urlParams.get('from') || null,
        to: urlParams.get('to') || null,
        onlyActiveRegisters: urlParams.get('onlyActiveRegisters') === 'true',
        departmentIds: urlParams.getAll('departmentIds'),
        byIsApplied: urlParams.get('byIsApplied') === 'true' ? true : null,
        byIsApproved: urlParams.get('byIsApproved') === 'true' ? true : null,
        isDesc: urlParams.get('isDesc') === 'true',
        orderBy: urlParams.get('orderBy') ? +urlParams.get('orderBy')! : PenaltyOrderBy.ByDate,
      };
    } else if (storedFilter) {
      this.penaltyFilter = JSON.parse(storedFilter);

      if (this.penaltyFilter.from) this.penaltyFilter.from = new Date(this.penaltyFilter.from).toISOString().split('T')[0];
      if (this.penaltyFilter.to) this.penaltyFilter.to = new Date(this.penaltyFilter.to).toISOString().split('T')[0];

      const queryParams = this.querySupporter.getPenaltiesQueryParams(this.penaltyFilter);
      this.router.navigate([], { queryParams, replaceUrl: true }); 
    } else { 
      this.penaltyFilter = {
        pageNumber: 1,
        pageSize: 10,
        name: null,
        language: 0,
        from: null,
        to: null,
        onlyActiveRegisters: true,
        departmentIds: [],
        byIsApplied: null,
        byIsApproved: null,
        isDesc: true,
        orderBy: PenaltyOrderBy.ByDate
      };
      const queryParams = this.querySupporter.getPenaltiesQueryParams(this.penaltyFilter);
      this.router.navigate([], { queryParams, replaceUrl: true }); 
    }
  }

  private patchFormFromFilter() {
    this.filterForm.patchValue({
      from: this.penaltyFilter.from,
      to: this.penaltyFilter.to,
      onlyActiveRegisters: this.penaltyFilter.onlyActiveRegisters,
      byIsApplied: this.penaltyFilter.byIsApplied,
      byIsApproved: this.penaltyFilter.byIsApproved,
      orderBy: this.penaltyFilter.orderBy,
      isDesc: this.penaltyFilter.isDesc
    }, { emitEvent: false });

    this.departmentIdsFormArray.clear();
    this.penaltyFilter.departmentIds?.forEach(id => {
      this.departmentIdsFormArray.push(new FormControl(id));
    });
  }

  private loadPenalties() {
    const queryParams = this.querySupporter.getPenaltiesQueryParams(this.penaltyFilter);
    console.log("Final Penalty Filter State (before URL update):", this.penaltyFilter); 
    console.log("Generated Query Params for URL:", queryParams); 

    this.router.navigate([], { queryParams: queryParams, replaceUrl: true }); 

    this.isLoadingPenalties = true;
    
    this.penaltiesPage$ = this.penaltyService
      .getAllPenalties(this.penaltyFilter)
      .pipe(
        finalize(() => {
          this.isLoadingPenalties = false;
        }),
        tap(page => {
          this.penaltiesPageSnapshot = {
            ...page,
            currentPage: page.currentPage,
            hasPrevious: page.currentPage > 1,
            hasNext: page.currentPage < page.totalPages
          };
        })
      );
  }

  applyFilters() {
    this.updatePenaltyFilterFromForm();
    this.penaltyFilter.pageNumber = 1; 
    this.loadPenalties();
    if (this.isSmallScreen) { // إخفاء الـ overlay بعد تطبيق الفلاتر على الشاشات الصغيرة
      this.showFilters = false;
    }
  }

  clearAllFilters() {
    this.isInitializingForm = true; 
    this.filterForm.reset({
      from: null,
      to: null,
      onlyActiveRegisters: true,
      byIsApplied: null,
      byIsApproved: null,
      orderBy: PenaltyOrderBy.ByDate,
      isDesc: true
    }, { emitEvent: false }); 
    this.departmentIdsFormArray.clear();

    this.penaltyFilter.name = null;
    this.penaltyFilter.departmentIds = [];
    this.penaltyFilter.language = 0;

    this.updatePenaltyFilterFromForm();
    this.penaltyFilter.pageNumber = 1; 
    this.isInitializingForm = false; 
    this.loadPenalties();
    if (this.isSmallScreen) { // إخفاء الـ overlay بعد مسح الفلاتر على الشاشات الصغيرة
      this.showFilters = false;
    }
  }

  clearIsAppliedFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ byIsApplied: null }, { emitEvent: false });
    this.updatePenaltyFilterFromForm();
    this.isInitializingForm = false;
    this.loadPenalties();
  }

  clearIsApprovedFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ byIsApproved: null }, { emitEvent: false });
    this.updatePenaltyFilterFromForm();
    this.isInitializingForm = false;
    this.loadPenalties();
  }

  clearDateRangeFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ from: null, to: null }, { emitEvent: false });
    this.updatePenaltyFilterFromForm();
    this.isInitializingForm = false;
    this.loadPenalties();
  }

  clearOnlyActiveRegistersFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ onlyActiveRegisters: true }, { emitEvent: false });
    this.updatePenaltyFilterFromForm();
    this.isInitializingForm = false;
    this.loadPenalties();
  }

  clearDepartmentIdsFilter() {
    this.isInitializingForm = true;
    this.departmentIdsFormArray.clear();
    this.updatePenaltyFilterFromForm();
    this.isInitializingForm = false;
    this.loadPenalties();
  }

  clearOrderFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({
      orderBy: PenaltyOrderBy.ByDate,
      isDesc: true
    }, { emitEvent: false });
    this.updatePenaltyFilterFromForm();
    this.isInitializingForm = false;
    this.loadPenalties();
  }


  getNextPage() {
    if (this.penaltiesPageSnapshot.hasNext) {
      this.penaltyFilter.pageNumber!++;
      this.loadPenalties();
    }
  }

  getPreviousPage() {
    if (this.penaltiesPageSnapshot.hasPrevious) {
      this.penaltyFilter.pageNumber!--;
      this.loadPenalties();
    }
  }

  goToPage(page: number) {
    if (page !== this.penaltiesPageSnapshot.currentPage) {
      this.penaltyFilter.pageNumber = page;
      this.loadPenalties();
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

  // هذه الدالة ستتحكم في ظهور الـ overlay
  toggleFilters() {
    this.showFilters = !this.showFilters;
    // منع التمرير في الخلفية عند فتح الـ overlay
    if (this.showFilters && this.isSmallScreen) {
      document.body.style.overflow = 'hidden';
    } else {
      document.body.style.overflow = 'auto';
    }
  }

  toggleDateRange() {
    this.isDateRangeExpanded = !this.isDateRangeExpanded;
  }

  toggleOnlyActive() {
    this.isOnlyActiveExpanded = !this.isOnlyActiveExpanded;
  }

  toggleDepartments() {
    this.isDepartmentsExpanded = !this.isDepartmentsExpanded;
  }

  toggleIsAppliedFilter() {
    this.isAppliedExpanded = !this.isAppliedExpanded;
  }

  toggleIsApprovedFilter() {
    this.isApprovedExpanded = !this.isApprovedExpanded;
  }

  toggleOrder() {
    this.isOrderExpanded = !this.isOrderExpanded;
  }

  showAddPenaltyForm() {
    this.isAddPenaltyFormVisible = true;
  }


  navigateToDetails(penaltyId: string) {
    sessionStorage.setItem('penaltyFilter', JSON.stringify(this.penaltyFilter));
    this.router.navigate([`/app/penalties/details/${penaltyId}`]);
  }

  ngOnDestroy(): void {
    sessionStorage.setItem('penaltyFilter', JSON.stringify(this.penaltyFilter));
    this.filterFormSubscription?.unsubscribe();
    this.searchNameSubscription?.unsubscribe();
    document.body.style.overflow = 'auto'; // إعادة التمرير عند مغادرة الصفحة
  }
}
