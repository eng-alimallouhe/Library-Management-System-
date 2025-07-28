// src/app/components/hr/leaves/leave-details/leave-details.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs/operators';
import { Subscription, Observable } from 'rxjs';

// استيراد الخدمات والـ DTOs الخاصة بالإجازات
import { LeaveService } from '../../../../services/hr/leave.service';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LeaveDetailsDto } from '../../../../shared/models/hr/leaves/leave-details.dto';
import { LeaveStatus } from '../../../../shared/models/hr/leaves/leave-status.enum';
import { LeaveType } from '../../../../shared/models/hr/leaves/leave-type.enum';
import { Result } from '../../../../shared/models/result/result';
import { EditLeaveComponent } from '../update-leave/update-leave.component';

@Component({
  selector: 'app-leave-details',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    LoaderComponent,
    LocalDatePipe,
    ConfirmDialogComponent,
    AlterComponent,
    EditLeaveComponent
  ],
  templateUrl: './leaves-details.component.html',
  styleUrls: ['./leaves-details.component.css']
})
export class LeaveDetailsComponent implements OnInit, OnDestroy {
  leaveId: string | null = null;
  leaveDetails: LeaveDetailsDto | null = null;
  isLoading = true;
  private routeSubscription: Subscription | undefined;
  private leaveSubscription: Subscription | undefined;

  isEditLeaveFormVisible = false;

  showConfirmActionDialog: boolean = false;
  confirmDialogTitle: string = '';
  confirmDialogMessage: string = '';
  private actionToPerform: 'approve' | 'reject' | 'delete' | null = null;

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private leaveService: LeaveService,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    console.log('ngOnInit: LeaveDetailsComponent initialized.');
    this.routeSubscription = this.route.paramMap.subscribe(params => {
      this.leaveId = params.get('id');
      console.log('ngOnInit: Leave ID from route:', this.leaveId);
      if (this.leaveId) {
        this.loadLeaveDetails(this.leaveId);
      } else {
        console.error('Leave ID is missing in the URL.');
        this.isLoading = false;
        this.showAlertMessage(false, 'COMMON.ERROR_LOADING_DATA');
      }
    });
  }

  private loadLeaveDetails(id: string): void {
    this.isLoading = true;
    console.log('loadLeaveDetails: Fetching leave details for ID:', id);
    this.leaveSubscription = this.leaveService.getLeaveById(id)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          console.log('loadLeaveDetails: Loading finished.');
        })
      )
      .subscribe({
        next: (data) => {
          this.leaveDetails = data;
          console.log('loadLeaveDetails: Leave details received:', this.leaveDetails);
        },
        error: (err) => {
          console.error('Error loading leave details:', err);
          this.leaveDetails = null;
          this.showAlertMessage(false, 'COMMON.ERROR_LOADING_DATA');
        }
      });
  }

  getLeaveTypeString(type: LeaveType): string {
    return this.translate.instant('HR.LEAVES.LEAVES.TYPE.' + LeaveType[type].toUpperCase());
  }

  getLeaveStatusString(status: LeaveStatus): string {
    return this.translate.instant('HR.LEAVES.LEAVES.STATUS.' + LeaveStatus[status].toUpperCase());
  }

  onEdit(): void {
    this.isEditLeaveFormVisible = true;
    console.log('Edit leave form visibility set to:', this.isEditLeaveFormVisible);
  }

  onLeaveUpdated(): void {
    console.log('Leave updated successfully, reloading details...');
    if (this.leaveId) {
      this.loadLeaveDetails(this.leaveId);
    }
  }

  onApprove(): void {
    this.actionToPerform = 'approve';
    this.translate.get('HR.LEAVES.CONFIRM_APPROVE_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.LEAVES.CONFIRM_APPROVE_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  onReject(): void {
    this.actionToPerform = 'reject';
    this.translate.get('HR.LEAVES.CONFIRM_REJECT_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.LEAVES.CONFIRM_REJECT_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  onDelete(): void {
    this.actionToPerform = 'delete';
    this.translate.get('HR.LEAVES.CONFIRM_DELETE_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.LEAVES.CONFIRM_DELETE_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  confirmAction(): void {
    this.showConfirmActionDialog = false;
    if (!this.leaveId) {
      console.error('Leave ID is missing for action.');
      this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
      return;
    }

    let actionObservable: Observable<Result>;
    let successMessageKey: string;
    let errorMessageKey: string;

    switch (this.actionToPerform) {
      case 'approve':
        actionObservable = this.leaveService.approveRejectLeave(this.leaveId, true);
        successMessageKey = 'HR.LEAVES.APPROVE_SUCCESS';
        errorMessageKey = 'HR.LEAVES.APPROVE_FAILED';
        break;
      case 'reject':
        actionObservable = this.leaveService.approveRejectLeave(this.leaveId, false);
        successMessageKey = 'HR.LEAVES.REJECT_SUCCESS';
        errorMessageKey = 'HR.LEAVES.REJECT_FAILED';
        break;
      case 'delete':
        actionObservable = this.leaveService.deleteLeave(this.leaveId);
        successMessageKey = 'HR.LEAVES.DELETE_SUCCESS';
        errorMessageKey = 'HR.LEAVES.DELETE_FAILED';
        break;
      default:
        console.error('Unknown action:', this.actionToPerform);
        this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
        return;
    }

    this.isLoading = true;

    actionObservable.pipe(
      finalize(() => {
        this.isLoading = false;
      })
    ).subscribe({
      next: response => {
        this.isSuccessAlert = response.isSuccess;
        this.translate.get(response.statusKey || successMessageKey)
          .subscribe(translatedMessage => {
            this.alertMessage = translatedMessage;
            this.alertVisible = true;
          });
        if (response.isSuccess) {
          if (this.actionToPerform === 'delete') {
            setTimeout(() => {
              this.router.navigate(['/app/hr/leaves']);
            }, 1500);
          } else {
            this.loadLeaveDetails(this.leaveId!);
          }
        }
      },
      error: error => {
        console.error(`Error ${this.actionToPerform}ing leave:`, error);
        this.isSuccessAlert = false;
        this.translate.get(error.error?.statusKey || errorMessageKey)
          .subscribe(translatedMessage => {
            this.alertMessage = translatedMessage;
            this.alertVisible = true;
          });
      }
    });
  }

  onCancelAction(): void {
    this.showConfirmActionDialog = false;
    this.actionToPerform = null;
  }

  onAlertClosed(): void {
    this.alertVisible = false;
  }

  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.translate.get(messageKey).subscribe(translatedMessage => {
      this.alertMessage = translatedMessage;
      this.alertVisible = true;
    });
  }

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.leaveSubscription?.unsubscribe();
  }
}
