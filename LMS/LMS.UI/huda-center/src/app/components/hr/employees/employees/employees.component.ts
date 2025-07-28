// src/app/components/hr/employees/employees.component.ts

import { Component, HostListener, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { debounceTime, finalize, tap } from 'rxjs/operators';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe } from '@ngx-translate/core';
import { EmployeeService } from '../../../../services/hr/employee.service';
import { DepartmentService } from '../../../../services/hr/department.service';
import { StorageService } from '../../../../shared/services/storage.service';
import { EmployeeFilter } from '../../../../shared/filters/hr/employee.filter';
import { EmployeeOverViewDto } from '../../../../shared/models/hr/employee/employee-overview.dto';
import { DepartmentLookupDto } from '../../../../shared/models/hr/departments/department-lookup.dto';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';
import { Observable, Subscription } from 'rxjs'; // إضافة Subscription
import { SearchCommunicationService } from '../../../../shared/services/search-communication.service';
import { QuerySupporterService } from '../../../../shared/services/query-supporter.service';
import { AddEmployeeComponent } from "../add-employee/add-employee.component";
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';

@Component({
  selector: 'app-employees',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    RouterLink,
    AsyncPipe,
    LoaderComponent,
    AddEmployeeComponent
  ],
  templateUrl: './employees.component.html',
  styleUrls: ['./employees.component.css']
})
export class EmployeesComponent implements OnInit, OnDestroy {
  employeesPage$!: Observable<PagedResult<EmployeeOverViewDto>>;
  employeesPageSnapshot!: PagedResult<EmployeeOverViewDto>;
  isLoadingEmployees = false;

  filterForm!: FormGroup;
  employeeFilter!: EmployeeFilter;
  selectedDepartments: string[] = [];
  filteredDepartments: DepartmentLookupDto[] = [];

  showFilters = false; // للتحكم في رؤية الـ overlay
  isSmallScreen = false;

  isDateRangeExpanded = true;
  isOnlyActiveExpanded = true;
  isDepartmentExpanded = true;
  isLanguageExpanded = true;
  isEmployeeAddFormVisible = false;

  private filterFormSubscription: Subscription | undefined; // إضافة تعريف Subscription
  private searchNameSubscription: Subscription | undefined; // إضافة تعريف Subscription

  @HostListener('window:resize', ['$event'])
  onResize() {
    this.checkScreenSize();
  }

  constructor(
    private fb: FormBuilder,
    private employeeService: EmployeeService,
    private departmentService: DepartmentService,
    private router: Router,
    private storageService: StorageService,
    private route: ActivatedRoute,
    private querySupporter: QuerySupporterService,
    private searchService: SearchCommunicationService
  ) {
  }

  ngOnInit(): void {
    this.initializeForm();
    this.checkScreenSize(); // يجب أن يتم استدعاؤها مبكراً لتحديد isSmallScreen

    this.loadDepartments(); // تحميل الأقسام أولاً

    // بعد تحميل الأقسام، يمكن استعادة الفلاتر وتطبيقها
    this.restoreFilterFromSessionOrURL();
    this.patchFormFromFilter();
    this.listenToFilterFormChanges();
    this.listenToSearchName();
    this.loadEmployees();
  }

  private checkScreenSize(): void {
    this.isSmallScreen = window.innerWidth <= 991;
    // على الشاشات الكبيرة، الفلاتر تظهر دائماً (كـ sidebar)
    if (!this.isSmallScreen) {
      this.showFilters = true;
    } else {
      // على الشاشات الصغيرة، الفلاتر تكون مخفية افتراضياً في البداية
      this.showFilters = false;
    }
  }

  private initializeForm() {
    this.filterForm = this.fb.group({
      name: [''],
      from: [null],
      to: [null],
      onlyActiveRegisters: [true],
      language: [0]
    });
  }

  private patchFormFromFilter() {
    this.filterForm.patchValue({
      name: this.employeeFilter.name,
      from: this.employeeFilter.from,
      to: this.employeeFilter.to,
      onlyActiveRegisters: this.employeeFilter.onlyActiveRegisters,
      language: this.employeeFilter.language
    }, { emitEvent: false }); // منع إعادة تحميل البيانات عند patchValue

    // تحديث selectedDepartments من الفلتر المستعاد
    this.selectedDepartments = this.employeeFilter.departmentIds || [];
  }

  private restoreFilterFromSessionOrURL() {
    const storedFilter = sessionStorage.getItem('employeeFilter');
    const urlParams = this.route.snapshot.queryParamMap;

    if (urlParams.keys.length > 0) {
      this.employeeFilter = {
        pageNumber: +(urlParams.get('pageNumber') || 1),
        pageSize: +(urlParams.get('pageSize') || 10),
        name: urlParams.get('name') || null,
        language: +(urlParams.get('language') || 0),
        from: urlParams.get('from') || null,
        to: urlParams.get('to') || null,
        onlyActiveRegisters: urlParams.get('onlyActiveRegisters') === 'true',
        departmentIds: urlParams.getAll('departmentIds'),
      };
    } else if (storedFilter) {
      this.employeeFilter = JSON.parse(storedFilter);
      sessionStorage.removeItem('employeeFilter');

      // تحويل التواريخ إذا كانت موجودة
      if (this.employeeFilter.from) this.employeeFilter.from = new Date(this.employeeFilter.from).toISOString().split('T')[0];
      if (this.employeeFilter.to) this.employeeFilter.to = new Date(this.employeeFilter.to).toISOString().split('T')[0];

      const queryParams = this.querySupporter.getEmployeesQueryParameters(this.employeeFilter);
      this.router.navigate([], { queryParams, replaceUrl: true });
    } else {
      this.employeeFilter = {
        pageNumber: 1,
        pageSize: 10,
        name: null,
        language: 0,
        departmentIds: [],
        from: null,
        to: null,
        onlyActiveRegisters: true,
      };
      const queryParams = this.querySupporter.getEmployeesQueryParameters(this.employeeFilter);
      this.router.navigate([], { queryParams, replaceUrl: true });
    }
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
        this.updateEmployeeFilterFromForm();
        this.loadEmployees();
      });
  }

  private updateEmployeeFilterFromForm() {
    const formValues = this.filterForm.value;
    this.employeeFilter = {
      ...this.employeeFilter,
      name: formValues.name || null,
      from: formValues.from || null,
      to: formValues.to || null,
      onlyActiveRegisters: formValues.onlyActiveRegisters,
      language: formValues.language,
      departmentIds: this.selectedDepartments, // تأكد من استخدام selectedDepartments
      pageNumber: 1 // إعادة تعيين رقم الصفحة عند تغيير الفلاتر
    };
  }

  private listenToSearchName() {
    if (this.searchNameSubscription) {
      this.searchNameSubscription.unsubscribe();
    }
    this.searchNameSubscription = this.searchService.nameSearch$.subscribe(name => {
      this.employeeFilter.name = name ? name : null;
      this.employeeFilter.pageNumber = 1;
      this.loadEmployees();
    });
  }

  private loadEmployees() {
    const queryParams = this.querySupporter.getEmployeesQueryParameters(this.employeeFilter);

    this.router.navigate([], { queryParams: queryParams, replaceUrl: true });

    this.isLoadingEmployees = true;
    this.employeesPage$ = this.employeeService
      .getAllEmployees(this.employeeFilter)
      .pipe(
        finalize(() => this.isLoadingEmployees = false),
        tap(page => {
          this.employeesPageSnapshot = {
            ...page,
            currentPage: page.currentPage, // تأكد من استخدام pageNumber هنا
            hasPrevious: page.currentPage > 1,
            hasNext: page.currentPage < page.totalPages
          };
        })
      );
  }

  private loadDepartments() {
    this.departmentService.getAvaliableDepartments(null).subscribe(depts => {
      this.filteredDepartments = depts;
    });
  }

  applyFilters() {
    this.updateEmployeeFilterFromForm();
    this.employeeFilter.pageNumber = 1;
    this.loadEmployees();
    if (this.isSmallScreen) { // إخفاء الـ overlay بعد تطبيق الفلاتر على الشاشات الصغيرة
      this.showFilters = false;
      document.body.style.overflow = 'auto'; // إعادة التمرير
    }
  }

  clearFilters() {
    this.filterForm.reset({
      name: '',
      from: null,
      to: null,
      onlyActiveRegisters: true,
      language: 0
    }, { emitEvent: false }); // منع إعادة تحميل البيانات عند reset

    this.selectedDepartments = []; // مسح الأقسام المختارة

    this.updateEmployeeFilterFromForm(); // تحديث الفلتر بعد reset
    this.employeeFilter.pageNumber = 1;
    this.loadEmployees();
    if (this.isSmallScreen) { // إخفاء الـ overlay بعد مسح الفلاتر على الشاشات الصغيرة
      this.showFilters = false;
      document.body.style.overflow = 'auto'; // إعادة التمرير
    }
  }

  // دوال مسح الفلاتر الفردية
  clearDateRangeFilter() {
    this.filterForm.patchValue({ from: null, to: null }, { emitEvent: false });
    this.updateEmployeeFilterFromForm();
    this.loadEmployees();
  }

  clearOnlyActiveRegistersFilter() {
    this.filterForm.patchValue({ onlyActiveRegisters: true }, { emitEvent: false });
    this.updateEmployeeFilterFromForm();
    this.loadEmployees();
  }

  clearDepartmentFilter() {
    this.selectedDepartments = [];
    this.updateEmployeeFilterFromForm();
    this.loadEmployees();
  }

  clearLanguageFilter() {
    this.filterForm.patchValue({ language: 0 }, { emitEvent: false });
    this.updateEmployeeFilterFromForm();
    this.loadEmployees();
  }


  onDepartmentToggle(dept: DepartmentLookupDto) {
    if (!this.selectedDepartments.includes(dept.departmentId)) {
      this.selectedDepartments.push(dept.departmentId);
    } else {
      this.selectedDepartments = this.selectedDepartments.filter(d => d !== dept.departmentId);
    }
    this.updateEmployeeFilterFromForm(); // تحديث الفلتر بعد التغيير
    this.loadEmployees(); // إعادة تحميل الموظفين
  }

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

  toggleDepartment() {
    this.isDepartmentExpanded = !this.isDepartmentExpanded;
  }

  toggleLanguage() {
    this.isLanguageExpanded = !this.isLanguageExpanded;
  }

  getNextPage() {
    if (this.employeesPageSnapshot.hasNext) {
      this.employeeFilter.pageNumber!++;
      this.loadEmployees();
    }
  }

  getPreviousPage() {
    if (this.employeesPageSnapshot.hasPrevious) {
      this.employeeFilter.pageNumber!--;
      this.loadEmployees();
    }
  }

  goToPage(page: number) {
    if (page !== this.employeesPageSnapshot.currentPage) {
      this.employeeFilter.pageNumber = page;
      this.loadEmployees();
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

  showEmployeeAddForm() {
    this.isEmployeeAddFormVisible = true;
  }

  navigateToDetails(employeeId: string) {
    sessionStorage.setItem('employeeFilter', JSON.stringify(this.employeeFilter));
    this.router.navigate([`/app/hr/employees/${employeeId}`]);
  }

  ngOnDestroy() {
    sessionStorage.setItem('employeeFilter', JSON.stringify(this.employeeFilter));
    this.filterFormSubscription?.unsubscribe();
    this.searchNameSubscription?.unsubscribe();
    document.body.style.overflow = 'auto'; // إعادة التمرير عند مغادرة الصفحة
  }
}
