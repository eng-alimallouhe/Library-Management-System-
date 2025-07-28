// src/app/components/hr/employees/employee-leaves-history/employee-leaves-history.component.ts

import { Component, Input, OnInit, OnDestroy } from '@angular/core';
import { CommonModule, AsyncPipe } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { Observable, Subscription } from 'rxjs';
import { finalize, tap } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

// استيراد الخدمات والـ DTOs الخاصة بالإجازات
import { LeaveService } from '../../../../services/hr/leave.service';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { LocalDatePipe } from '../../../../pipes/local-date.pipe';

import { AddLeaveComponent } from '../../leaves/add-leave/add-leave.component'; // <--- إضافة هذا الاستيراد
import { LeaveOverviewDto } from '../../../../shared/models/hr/leave-view.dto';
import { LeaveStatus } from '../../../../shared/models/hr/leaves/leave-status.enum';
import { LeaveType } from '../../../../shared/models/hr/leaves/leave-type.enum';
import { StorageService, userData } from '../../../../shared/services/storage.service';

@Component({
  selector: 'app-employee-leaves-history',
  standalone: true,
  imports: [
    CommonModule,
    AsyncPipe,
    RouterLink,
    TranslatePipe,
    LoaderComponent,
    LocalDatePipe,
    AddLeaveComponent // <--- إضافة المكون هنا
  ],
  templateUrl: './employee-leave-history.component.html',
  styleUrls: ['./employee-leave-history.component.css']
})
export class EmployeeLeavesHistoryComponent implements OnInit, OnDestroy {

  employeeId!: string;

  leavesPage$!: Observable<PagedResult<LeaveOverviewDto>>;
  isLoadingLeaves = true;

  isAddLeaveFormVisible = false;

  private leavesSubscription: Subscription | undefined;

  constructor(
    private leaveService: LeaveService,
    private router: Router,
    private translate: TranslateService,
    private storageService: StorageService
  ) { }

  ngOnInit(): void {
    this.employeeId = this.storageService.getItem(userData.id) || '1096c9a6-8e63-4144-95b6-261412bff219';
    if (this.employeeId) {
      this.loadEmployeeLeaves();
    } else {
      console.error('Employee ID is missing for loading leave history.');
      this.isLoadingLeaves = false;
    }
  }

  private loadEmployeeLeaves(): void {
    this.isLoadingLeaves = true;
    this.leavesPage$ = this.leaveService.getEmployeeLeaves(this.employeeId)
      .pipe(
        finalize(() => {
          this.isLoadingLeaves = false;
          console.log('Employee leaves loaded.');
        }),
      );
  }

  getLeaveTypeString(type: LeaveType): string {
    return this.translate.instant('HR.LEAVES.LEAVES.TYPE.' + LeaveType[type].toUpperCase());
  }

  getLeaveStatusString(status: LeaveStatus): string {
    return this.translate.instant('HR.LEAVES.LEAVES.STATUS.' + LeaveStatus[status].toUpperCase());
  }

  showAddLeaveForm(): void {
    console.log('error');
    
    this.isAddLeaveFormVisible = true;
  }

  onLeaveAdded(): void {
    this.loadEmployeeLeaves();
  }

  navigateToLeaveDetails(leaveId: string): void {
    this.router.navigate([`/app/hr/leaves/${leaveId}`]);
  }

  ngOnDestroy(): void {
    this.leavesSubscription?.unsubscribe();
  }
}
