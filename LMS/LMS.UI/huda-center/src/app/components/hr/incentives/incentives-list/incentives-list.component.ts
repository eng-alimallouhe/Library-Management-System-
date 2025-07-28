// src/app/components/hr/incentives/incentives-list/incentives-list.component.ts

import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormArray, FormControl, AbstractControl } from '@angular/forms'; // إضافة AbstractControl
import { ActivatedRoute, Router, RouterLink, Params } from '@angular/router';
import { debounceTime, finalize, tap } from 'rxjs/operators';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable, Subscription } from 'rxjs';

import { AddIncentiveComponent } from '../add-incentive/add-incentive.component';
import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { DepartmentService } from '../../../../services/hr/department.service';
import { IncentiveService } from '../../../../services/hr/incentive.service';
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { IncentiveFilter, IncentiveOrderBy } from '../../../../shared/filters/hr/incentive.filter';
import { AvaliableDepartmentDto } from '../../../../shared/models/hr/departments/avaliable-departments.dto';
import { IncentiveOverviewDto } from '../../../../shared/models/hr/incentives/incentive-overview.dto';
import { Result } from '../../../../shared/models/result/result';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';
import { QuerySupporterService } from '../../../../shared/services/query-supporter.service';
import { SearchCommunicationService } from '../../../../shared/services/search-communication.service';

@Component({
  selector: 'app-incentives-list', // تم تغيير السيلكتور
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
  templateUrl: './incentives-list.component.html',
  styleUrls: ['./incentives-list.component.css']
})
export class IncentivesListComponent implements OnInit, OnDestroy { // تم تغيير اسم الكلاس

  incentivesPage$!: Observable<PagedResult<IncentiveOverviewDto>>;
  incentivesPageSnapshot!: PagedResult<IncentiveOverviewDto>;
  isLoadingIncentives = true; // تم تغيير الاسم

  filterForm!: FormGroup;
  incentiveFilter!: IncentiveFilter; // تم تغيير الاسم

  showFilters = false;
  isSmallScreen = false;

  isDateRangeExpanded = true;
  isOnlyActiveExpanded = true;
  isDepartmentsExpanded = true;
  isPaidExpanded = true; // تم تغيير الاسم من isAppliedExpanded
  isApprovedExpanded = true;
  isOrderExpanded = true;

  availableDepartments: AvaliableDepartmentDto[] = [];
  isLoadingDepartments = true;

  private filterFormSubscription: Subscription | undefined;
  private searchNameSubscription: Subscription | undefined;
  private isInitializingForm: boolean = true;


  incentiveOrderByOptions = [ // تم تغيير الاسم
    { value: IncentiveOrderBy.ByEmployeeName, label: 'HR.INCENTIVES.ORDER_BY.BY_EMPLOYEE_NAME' },
    { value: IncentiveOrderBy.ByDate, label: 'HR.INCENTIVES.ORDER_BY.BY_DATE' },
    { value: IncentiveOrderBy.ByReason, label: 'HR.INCENTIVES.ORDER_BY.BY_REASON' },
    { value: IncentiveOrderBy.ByIsPaid, label: 'HR.INCENTIVES.ORDER_BY.BY_IS_PAID' }, // تم تغيير الاسم
    { value: IncentiveOrderBy.ByIsApproved, label: 'HR.INCENTIVES.ORDER_BY.BY_IS_APPROVED' },
  ];

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  constructor(
    private fb: FormBuilder,
    private incentiveService: IncentiveService, // تم تغيير الخدمة
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
          this.loadIncentives(); // تم تغيير الدالة
        })
      )
      .subscribe({
        next: (deps) => {
          this.availableDepartments = deps;
        },
        error: (err) => {
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
      byIsPaid: [null], 
      byIsApproved: [null],
      orderBy: [IncentiveOrderBy.ByDate],
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
          this.updateIncentiveFilterFromForm(); // تم تغيير الدالة
          this.loadIncentives(); // تم تغيير الدالة
        }
      });
  }

  private updateIncentiveFilterFromForm() { // تم تغيير الاسم
    const formValues = this.filterForm.value;

    this.incentiveFilter = { // تم تغيير الاسم
      ...this.incentiveFilter,
      from: formValues.from || null,
      to: formValues.to || null,
      onlyActiveRegisters: formValues.onlyActiveRegisters,
      language: 0,
      departmentIds: formValues.departmentIds,
      byIsPaid: formValues.byIsPaid, // تم تغيير الاسم
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
      this.incentiveFilter.name = name ? name : null; // تم تغيير الاسم
      this.incentiveFilter.pageNumber = 1; // تم تغيير الاسم
      this.loadIncentives(); // تم تغيير الدالة
    });
  }

  private restoreFilterFromSessionOrURL() {
    const storedFilter = sessionStorage.getItem('incentiveFilter'); // تم تغيير اسم الـ key
    const urlParams = this.route.snapshot.queryParamMap;

    if (urlParams.keys.length > 0) {
      this.incentiveFilter = { // تم تغيير الاسم
        pageNumber: +(urlParams.get('pageNumber') || 1),
        pageSize: +(urlParams.get('pageSize') || 10),
        name: urlParams.get('name') || null,
        language: +(urlParams.get('language') || 0),
        from: urlParams.get('from') || null,
        to: urlParams.get('to') || null,
        onlyActiveRegisters: urlParams.get('onlyActiveRegisters') === 'true',
        departmentIds: urlParams.getAll('departmentIds'),
        byIsPaid: urlParams.get('byIsPaid') === 'true' ? true : (urlParams.get('byIsPaid') === 'false' ? false : null), // تم التعديل للتعامل مع null
        byIsApproved: urlParams.get('byIsApproved') === 'true' ? true : (urlParams.get('byIsApproved') === 'false' ? false : null), // تم التعديل للتعامل مع null
        isDesc: urlParams.get('isDesc') === 'true',
        orderBy: urlParams.get('orderBy') ? +urlParams.get('orderBy')! : IncentiveOrderBy.ByDate, // تم تغيير الاسم
      };
    } else if (storedFilter) {
      this.incentiveFilter = JSON.parse(storedFilter); // تم تغيير الاسم

      if (this.incentiveFilter.from) this.incentiveFilter.from = new Date(this.incentiveFilter.from).toISOString().split('T')[0];
      if (this.incentiveFilter.to) this.incentiveFilter.to = new Date(this.incentiveFilter.to).toISOString().split('T')[0];

      const queryParams = this.querySupporter.getIncentivesQueryParams(this.incentiveFilter); // تم تغيير الدالة
      this.router.navigate([], { queryParams, replaceUrl: true });
    } else {
      this.incentiveFilter = { // تم تغيير الاسم
        pageNumber: 1,
        pageSize: 10,
        name: null,
        language: 0,
        from: null,
        to: null,
        onlyActiveRegisters: true,
        departmentIds: [],
        byIsPaid: null, // تم تغيير الاسم
        byIsApproved: null,
        isDesc: true,
        orderBy: IncentiveOrderBy.ByDate // تم تغيير الاسم
      };
      const queryParams = this.querySupporter.getIncentivesQueryParams(this.incentiveFilter); // تم تغيير الدالة
      this.router.navigate([], { queryParams, replaceUrl: true });
    }
  }

  private patchFormFromFilter() {
    this.filterForm.patchValue({
      from: this.incentiveFilter.from,
      to: this.incentiveFilter.to,
      onlyActiveRegisters: this.incentiveFilter.onlyActiveRegisters,
      byIsPaid: this.incentiveFilter.byIsPaid, // تم تغيير الاسم
      byIsApproved: this.incentiveFilter.byIsApproved,
      orderBy: this.incentiveFilter.orderBy,
      isDesc: this.incentiveFilter.isDesc
    }, { emitEvent: false });

    this.departmentIdsFormArray.clear();
    this.incentiveFilter.departmentIds?.forEach(id => {
      this.departmentIdsFormArray.push(new FormControl(id));
    });
  }

  private loadIncentives() { // تم تغيير الاسم
    const queryParams = this.querySupporter.getIncentivesQueryParams(this.incentiveFilter); // تم تغيير الدالة
    console.log("Final Incentive Filter State (before URL update):", this.incentiveFilter);
    console.log("Generated Query Params for URL:", queryParams);

    this.router.navigate([], { queryParams: queryParams, replaceUrl: true });

    this.isLoadingIncentives = true; // تم تغيير الاسم

    this.incentivesPage$ = this.incentiveService // تم تغيير الخدمة
      .getAllIncentives(this.incentiveFilter) // تم تغيير الدالة
      .pipe(
        finalize(() => {
          this.isLoadingIncentives = false; // تم تغيير الاسم
        }),
        tap(page => {
          this.incentivesPageSnapshot = { // تم تغيير الاسم
            ...page,
            currentPage: page.currentPage,
            hasPrevious: page.currentPage > 1,
            hasNext: page.currentPage < page.totalPages
          };
        })
      );
  }

  applyFilters() {
    this.updateIncentiveFilterFromForm(); // تم تغيير الدالة
    this.incentiveFilter.pageNumber = 1; // تم تغيير الاسم
    this.loadIncentives(); // تم تغيير الدالة
    if (this.isSmallScreen) {
      this.showFilters = false;
    }
  }

  clearAllFilters() {
    this.isInitializingForm = true;
    this.filterForm.reset({
      from: null,
      to: null,
      onlyActiveRegisters: true,
      byIsPaid: null, // تم تغيير الاسم
      byIsApproved: null,
      orderBy: IncentiveOrderBy.ByDate, // تم تغيير الاسم
      isDesc: true
    }, { emitEvent: false });
    this.departmentIdsFormArray.clear();

    this.incentiveFilter.name = null; // تم تغيير الاسم
    this.incentiveFilter.departmentIds = []; // تم تغيير الاسم
    this.incentiveFilter.language = 0; // تم تغيير الاسم

    this.updateIncentiveFilterFromForm(); // تم تغيير الدالة
    this.incentiveFilter.pageNumber = 1; // تم تغيير الاسم
    this.isInitializingForm = false;
    this.loadIncentives(); // تم تغيير الدالة
    if (this.isSmallScreen) {
      this.showFilters = false;
    }
  }

  clearIsPaidFilter() { // تم تغيير الاسم
    this.isInitializingForm = true;
    this.filterForm.patchValue({ byIsPaid: null }, { emitEvent: false }); // تم تغيير الاسم
    this.updateIncentiveFilterFromForm(); // تم تغيير الدالة
    this.isInitializingForm = false;
    this.loadIncentives(); // تم تغيير الدالة
  }

  clearIsApprovedFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ byIsApproved: null }, { emitEvent: false });
    this.updateIncentiveFilterFromForm();
    this.isInitializingForm = false;
    this.loadIncentives();
  }

  clearDateRangeFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ from: null, to: null }, { emitEvent: false });
    this.updateIncentiveFilterFromForm();
    this.isInitializingForm = false;
    this.loadIncentives();
  }

  clearOnlyActiveRegistersFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ onlyActiveRegisters: true }, { emitEvent: false });
    this.updateIncentiveFilterFromForm();
    this.isInitializingForm = false;
    this.loadIncentives();
  }

  clearDepartmentIdsFilter() {
    this.isInitializingForm = true;
    this.departmentIdsFormArray.clear();
    this.updateIncentiveFilterFromForm();
    this.isInitializingForm = false;
    this.loadIncentives();
  }

  clearOrderFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({
      orderBy: IncentiveOrderBy.ByDate, // تم تغيير الاسم
      isDesc: true
    }, { emitEvent: false });
    this.updateIncentiveFilterFromForm();
    this.isInitializingForm = false;
    this.loadIncentives();
  }

  getNextPage() {
    if (this.incentivesPageSnapshot.hasNext) { // تم تغيير الاسم
      this.incentiveFilter.pageNumber!++; // تم تغيير الاسم
      this.loadIncentives(); // تم تغيير الدالة
    }
  }

  getPreviousPage() {
    if (this.incentivesPageSnapshot.hasPrevious) { // تم تغيير الاسم
      this.incentiveFilter.pageNumber!--; // تم تغيير الاسم
      this.loadIncentives(); // تم تغيير الدالة
    }
  }

  goToPage(page: number) {
    if (page !== this.incentivesPageSnapshot.currentPage) { // تم تغيير الاسم
      this.incentiveFilter.pageNumber = page; // تم تغيير الاسم
      this.loadIncentives(); // تم تغيير الدالة
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

  toggleIsPaidFilter() { // تم تغيير الاسم
    this.isPaidExpanded = !this.isPaidExpanded;
  }

  toggleIsApprovedFilter() {
    this.isApprovedExpanded = !this.isApprovedExpanded;
  }

  toggleOrder() {
    this.isOrderExpanded = !this.isOrderExpanded;
  }


  onIncentiveAdded(): void { // تم تغيير الاسم
    this.loadIncentives(); // تم تغيير الدالة
  }

  navigateToDetails(incentiveId: string) { // تم تغيير الاسم
    sessionStorage.setItem('incentiveFilter', JSON.stringify(this.incentiveFilter)); // تم تغيير اسم الـ key
    this.router.navigate([`/app/hr/incentives/${incentiveId}`]); // تم تغيير المسار
  }


  getControl(name: string): AbstractControl | null {
    return this.filterForm.get(name);
  }

  ngOnDestroy(): void {
    sessionStorage.setItem('incentiveFilter', JSON.stringify(this.incentiveFilter)); // تم تغيير اسم الـ key
    this.filterFormSubscription?.unsubscribe();
    this.searchNameSubscription?.unsubscribe();
    document.body.style.overflow = 'auto';
  }
}
