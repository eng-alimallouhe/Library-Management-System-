// src/app/components/hr/leaves/leaves-list/leaves-list.component.ts

import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule, FormArray, FormControl, AbstractControl } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink, Params } from '@angular/router';
import { debounceTime, finalize, tap } from 'rxjs/operators';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { Observable, Subscription } from 'rxjs';

// استيراد الخدمات والـ DTOs والفلاتر الخاصة بالإجازات
import { LeaveService } from '../../../../services/hr/leave.service';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { QuerySupporterService } from '../../../../shared/services/query-supporter.service';
import { SearchCommunicationService } from '../../../../shared/services/search-communication.service';
import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { DepartmentService } from '../../../../services/hr/department.service';
import { AvaliableDepartmentDto } from '../../../../shared/models/hr/departments/avaliable-departments.dto';
import { LeaveFilter, LeaveOrderBy } from '../../../../shared/filters/hr/leave.filter';
import { LeaveOverviewDto } from '../../../../shared/models/hr/leave-view.dto';
import { LeaveStatus } from '../../../../shared/models/hr/leaves/leave-status.enum';
import { LeaveType } from '../../../../shared/models/hr/leaves/leave-type.enum';
import { AlterComponent } from "../../../../shared/components/alter/alter.component";

@Component({
  selector: 'app-leaves-list',
  standalone: true,
  imports: [
    TranslatePipe,
    AsyncPipe,
    CommonModule,
    ReactiveFormsModule,
    LoaderComponent,
    LocalDatePipe,
    RouterLink,
    AlterComponent
],
  templateUrl: './leaves-list.component.html',
  styleUrls: ['./leaves-list.component.css']
})
export class LeavesListComponent implements OnInit, OnDestroy {

  leavesPage$!: Observable<PagedResult<LeaveOverviewDto>>;
  leavesPageSnapshot!: PagedResult<LeaveOverviewDto>;
  isLoadingLeaves = true;

  filterForm!: FormGroup;
  leaveFilter!: LeaveFilter;

  showFilters = false;
  isSmallScreen = false;

  isDateRangeExpanded = true;
  isOnlyActiveExpanded = true;
  isDepartmentsExpanded = true;
  isPaidExpanded = true;
  isApprovedExpanded = true; // في الإجازات هي Status
  isTypeExpanded = true;
  isOrderExpanded = true;

  availableDepartments: AvaliableDepartmentDto[] = [];
  isLoadingDepartments = true;

  private filterFormSubscription: Subscription | undefined;
  private searchNameSubscription: Subscription | undefined;
  private isInitializingForm: boolean = true;

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  leaveOrderByOptions = [
    { value: LeaveOrderBy.ByEmployeeName, label: 'HR.LEAVES.LEAVES.ORDER_BY.BY_EMPLOYEE_NAME' }, // تم التعديل
    { value: LeaveOrderBy.ByStartDate, label: 'HR.LEAVES.LEAVES.ORDER_BY.BY_START_DATE' },     // تم التعديل
    { value: LeaveOrderBy.ByEndDate, label: 'HR.LEAVES.LEAVES.ORDER_BY.BY_END_DATE' },         // تم التعديل
    { value: LeaveOrderBy.ByStatus, label: 'HR.LEAVES.LEAVES.ORDER_BY.BY_STATUS' },           // تم التعديل
    { value: LeaveOrderBy.ByType, label: 'HR.LEAVES.LEAVES.ORDER_BY.BY_TYPE' },               // تم التعديل
    { value: LeaveOrderBy.ByCreatedAt, label: 'HR.LEAVES.LEAVES.ORDER_BY.BY_CREATED_AT' },     // تم التعديل
  ];

  leaveStatusOptions = [
    { value: LeaveStatus.Pending, label: 'HR.LEAVES.LEAVES.STATUS.PENDING' },   // تم التعديل
    { value: LeaveStatus.Approved, label: 'HR.LEAVES.LEAVES.STATUS.APPROVED' }, // تم التعديل
    { value: LeaveStatus.Rejected, label: 'HR.LEAVES.LEAVES.STATUS.REJECTED' }, // تم التعديل
    { value: LeaveStatus.Cancelled, label: 'HR.LEAVES.LEAVES.STATUS.CANCELLED' }, // تم التعديل
  ];

  leaveTypeOptions = [
    { value: LeaveType.Annual, label: 'HR.LEAVES.LEAVES.TYPE.ANNUAL' },         // تم التعديل
    { value: LeaveType.Sick, label: 'HR.LEAVES.LEAVES.TYPE.SICK' },             // تم التعديل
    { value: LeaveType.Unpaid, label: 'HR.LEAVES.LEAVES.TYPE.UNPAID' },         // تم التعديل
    { value: LeaveType.Maternity, label: 'HR.LEAVES.LEAVES.TYPE.MATERNITY' },   // تم التعديل
    { value: LeaveType.Paternity, label: 'HR.LEAVES.LEAVES.TYPE.PATERNITY' },   // تم التعديل
    { value: LeaveType.Marriage, label: 'HR.LEAVES.LEAVES.TYPE.MARRIAGE' },     // تم التعديل
    { value: LeaveType.Death, label: 'HR.LEAVES.LEAVES.TYPE.DEATH' },           // تم التعديل
    { value: LeaveType.Study, label: 'HR.LEAVES.LEAVES.TYPE.STUDY' },           // تم التعديل
    { value: LeaveType.Hajj, label: 'HR.LEAVES.LEAVES.TYPE.HAJJ' },             // تم التعديل
    { value: LeaveType.OfficialLeave, label: 'HR.LEAVES.LEAVES.TYPE.OFFICIALLEAVE' }, // تم التعديل
    { value: LeaveType.Other, label: 'HR.LEAVES.LEAVES.TYPE.OTHER' },           // تم التعديل
  ];

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  constructor(
    private fb: FormBuilder,
    private leaveService: LeaveService,
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
          this.loadLeaves();
        })
      )
      .subscribe({
        next: (deps) => {
          this.availableDepartments = deps;
        },
        error: (err) => {
          console.error('Error loading available departments:', err);
          this.showAlertMessage(false, 'COMMON.ERROR_LOADING_DATA');
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
      status: [null],
      type: [null],
      isPaid: [null],
      orderBy: [LeaveOrderBy.ByCreatedAt],
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
          this.updateLeaveFilterFromForm();
          this.loadLeaves();
        }
      });
  }

  private updateLeaveFilterFromForm() {
    const formValues = this.filterForm.value;

    this.leaveFilter = {
      ...this.leaveFilter,
      from: formValues.from || null,
      to: formValues.to || null,
      status: formValues.status,
      type: formValues.type,
      isPaid: formValues.isPaid,
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
      this.leaveFilter.name = name ? name : null;
      this.leaveFilter.pageNumber = 1;
      this.loadLeaves();
    });
  }

  private restoreFilterFromSessionOrURL() {
    const storedFilter = sessionStorage.getItem('leaveFilter');
    const urlParams = this.route.snapshot.queryParamMap;

    if (urlParams.keys.length > 0) {
      this.leaveFilter = {
        pageNumber: +(urlParams.get('pageNumber') || 1),
        pageSize: +(urlParams.get('pageSize') || 10),
        name: urlParams.get('name') || null,
        from: urlParams.get('from') || null,
        to: urlParams.get('to') || null,
        status: urlParams.get('status') ? +urlParams.get('status')! : null,
        type: urlParams.get('type') ? +urlParams.get('type')! : null,
        isPaid: urlParams.get('isPaid') === 'true' ? true : (urlParams.get('isPaid') === 'false' ? false : null),
        isDesc: urlParams.get('isDesc') === 'true',
        orderBy: urlParams.get('orderBy') ? +urlParams.get('orderBy')! : LeaveOrderBy.ByCreatedAt,
        language: 0
      };
    } else if (storedFilter) {
      this.leaveFilter = JSON.parse(storedFilter);

      if (this.leaveFilter.from) this.leaveFilter.from = new Date(this.leaveFilter.from).toISOString().split('T')[0];
      if (this.leaveFilter.to) this.leaveFilter.to = new Date(this.leaveFilter.to).toISOString().split('T')[0];

      const queryParams = this.querySupporter.getLeavesQueryParams(this.leaveFilter);
      this.router.navigate([], { queryParams, replaceUrl: true });
    } else {
      this.leaveFilter = {
        pageNumber: 1,
        pageSize: 10,
        name: null,
        from: null,
        to: null,
        status: null,
        type: null,
        isPaid: null,
        isDesc: true,
        orderBy: LeaveOrderBy.ByCreatedAt,
        language: 0
      };
      const queryParams = this.querySupporter.getLeavesQueryParams(this.leaveFilter);
      this.router.navigate([], { queryParams, replaceUrl: true });
    }
  }

  private patchFormFromFilter() {
    this.filterForm.patchValue({
      from: this.leaveFilter.from,
      to: this.leaveFilter.to,
      status: this.leaveFilter.status,
      type: this.leaveFilter.type,
      isPaid: this.leaveFilter.isPaid,
      orderBy: this.leaveFilter.orderBy,
      isDesc: this.leaveFilter.isDesc
    }, { emitEvent: false });
  }

  private loadLeaves() {
    const queryParams = this.querySupporter.getLeavesQueryParams(this.leaveFilter);
    console.log("Final Leave Filter State (before URL update):", this.leaveFilter);
    console.log("Generated Query Params for URL:", queryParams);

    this.router.navigate([], { queryParams: queryParams, replaceUrl: true });

    this.isLoadingLeaves = true;

    this.leavesPage$ = this.leaveService
      .getAllLeaves(this.leaveFilter)
      .pipe(
        finalize(() => {
          this.isLoadingLeaves = false;
        }),
        tap(page => {
          this.leavesPageSnapshot = {
            ...page,
            currentPage: page.currentPage,
            hasPrevious: page.currentPage > 1,
            hasNext: page.currentPage < page.totalPages
          };
        })
      );
  }

  applyFilters() {
    this.updateLeaveFilterFromForm();
    this.leaveFilter.pageNumber = 1;
    this.loadLeaves();
    if (this.isSmallScreen) {
      this.showFilters = false;
    }
  }

  clearAllFilters() {
    this.isInitializingForm = true;
    this.filterForm.reset({
      from: null,
      to: null,
      status: null,
      type: null,
      isPaid: null,
      orderBy: LeaveOrderBy.ByCreatedAt,
      isDesc: true
    }, { emitEvent: false });

    this.leaveFilter.name = null;

    this.updateLeaveFilterFromForm();
    this.leaveFilter.pageNumber = 1;
    this.isInitializingForm = false;
    this.loadLeaves();
    if (this.isSmallScreen) {
      this.showFilters = false;
    }
  }

  clearIsPaidFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ isPaid: null }, { emitEvent: false });
    this.updateLeaveFilterFromForm();
    this.isInitializingForm = false;
    this.loadLeaves();
  }

  clearStatusFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ status: null }, { emitEvent: false });
    this.updateLeaveFilterFromForm();
    this.isInitializingForm = false;
    this.loadLeaves();
  }

  clearTypeFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ type: null }, { emitEvent: false });
    this.updateLeaveFilterFromForm();
    this.isInitializingForm = false;
    this.loadLeaves();
  }

  clearDateRangeFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({ from: null, to: null }, { emitEvent: false });
    this.updateLeaveFilterFromForm();
    this.isInitializingForm = false;
    this.loadLeaves();
  }

  clearOrderFilter() {
    this.isInitializingForm = true;
    this.filterForm.patchValue({
      orderBy: LeaveOrderBy.ByCreatedAt,
      isDesc: true
    }, { emitEvent: false });
    this.updateLeaveFilterFromForm();
    this.isInitializingForm = false;
    this.loadLeaves();
  }

  getNextPage() {
    if (this.leavesPageSnapshot.hasNext) {
      this.leaveFilter.pageNumber!++;
      this.loadLeaves();
    }
  }

  getPreviousPage() {
    if (this.leavesPageSnapshot.hasPrevious) {
      this.leaveFilter.pageNumber!--;
      this.loadLeaves();
    }
  }

  goToPage(page: number) {
    if (page !== this.leavesPageSnapshot.currentPage) {
      this.leaveFilter.pageNumber = page;
      this.loadLeaves();
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

  toggleIsPaidFilter() {
    this.isPaidExpanded = !this.isPaidExpanded;
  }

  toggleStatusFilter() {
    this.isApprovedExpanded = !this.isApprovedExpanded;
  }

  toggleTypeFilter() {
    this.isTypeExpanded = !this.isTypeExpanded;
  }

  toggleOrder() {
    this.isOrderExpanded = !this.isOrderExpanded;
  }

  navigateToDetails(leaveId: string) {
    sessionStorage.setItem('leaveFilter', JSON.stringify(this.leaveFilter));
    this.router.navigate([`/app/hr/leaves/${leaveId}`]);
  }

  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.translate.get(messageKey).subscribe(translatedMessage => {
      this.alertMessage = translatedMessage;
      this.alertVisible = true;
    });
  }

  onAlertClosed(): void {
    this.alertVisible = false;
  }

  getControl(name: string): AbstractControl | null {
    return this.filterForm.get(name);
  }

  ngOnDestroy(): void {
    sessionStorage.setItem('leaveFilter', JSON.stringify(this.leaveFilter));
    this.filterFormSubscription?.unsubscribe();
    this.searchNameSubscription?.unsubscribe();
    document.body.style.overflow = 'auto';
  }
}
