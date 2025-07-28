// src/app/components/hr/incentives/incentive-details/incentive-details.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs/operators';
import { Subscription, Observable } from 'rxjs'; // إضافة Observable

import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { IncentiveService } from '../../../../services/hr/incentive.service';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { IncentiveDetailsDto } from '../../../../shared/models/hr/incentives/incentive-details.dto';

import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component';
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { EditIncentiveComponent } from '../update-incentive/update-incentive.component';
import { Result } from '../../../../shared/models/results/result';

@Component({
  selector: 'app-incentive-details',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    LoaderComponent,
    LocalDatePipe,
    ConfirmDialogComponent,
    AlterComponent,
    EditIncentiveComponent
],
  templateUrl: './incentive-details.component.html',
  styleUrls: ['./incentive-details.component.css']
})
export class IncentiveDetailsComponent implements OnInit, OnDestroy {
  incentiveId: string | null = null;
  incentiveDetails: IncentiveDetailsDto | null = null;
  isLoading = true;
  private routeSubscription: Subscription | undefined;
  private incentiveSubscription: Subscription | undefined;

  isEditIncentiveFormVisible = false;

  // متغيرات Confirm Dialog
  showConfirmActionDialog: boolean = false;
  confirmDialogTitle: string = '';
  confirmDialogMessage: string = '';
  private actionToPerform: 'approve' | 'reject' | 'delete' | null = null; // إضافة 'delete'

  // متغيرات Alert
  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private incentiveService: IncentiveService,
    private sanitizer: DomSanitizer,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    console.log('ngOnInit: IncentiveDetailsComponent initialized.');
    this.routeSubscription = this.route.paramMap.subscribe(params => {
      this.incentiveId = params.get('id');
      console.log('ngOnInit: Incentive ID from route:', this.incentiveId);
      if (this.incentiveId) {
        this.loadIncentiveDetails(this.incentiveId);
      } else {
        console.error('Incentive ID is missing in the URL.');
        this.isLoading = false;
        this.showAlertMessage(false, 'COMMON.ERROR_LOADING_DATA');
      }
    });
  }

  private loadIncentiveDetails(id: string): void {
    this.isLoading = true;
    console.log('loadIncentiveDetails: Fetching incentive details for ID:', id);
    this.incentiveSubscription = this.incentiveService.getIncentiveById(id)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          console.log('loadIncentiveDetails: Loading finished.');
        })
      )
      .subscribe({
        next: (data) => {
          this.incentiveDetails = data;
          console.log('loadIncentiveDetails: Incentive details received:', this.incentiveDetails);
          if (!this.incentiveDetails.decisionFileUrl) {
            console.log('loadIncentiveDetails: No decision file URL found.');
          }
        },
        error: (err) => {
          console.error('Error loading incentive details:', err);
          this.incentiveDetails = null;
          this.showAlertMessage(false, 'COMMON.ERROR_LOADING_DATA');
        }
      });
  }

  get downloadLinkSafeUrl(): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(this.incentiveDetails?.decisionFileUrl || '');
  }

  onEdit(): void {
    this.isEditIncentiveFormVisible = true;
    console.log('Edit incentive form visibility set to:', this.isEditIncentiveFormVisible);
  }

  onIncentiveUpdated(): void {
    console.log('Incentive updated successfully, reloading details...');
    if (this.incentiveId) {
      this.loadIncentiveDetails(this.incentiveId);
    }
  }

  /**
   * @description Prepares and shows the confirmation dialog for approving an incentive.
   */
  onApprove(): void {
    this.actionToPerform = 'approve';
    this.translate.get('HR.INCENTIVES.CONFIRM_APPROVE_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.INCENTIVES.CONFIRM_APPROVE_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  /**
   * @description Prepares and shows the confirmation dialog for rejecting an incentive.
   */
  onReject(): void {
    this.actionToPerform = 'reject';
    this.translate.get('HR.INCENTIVES.CONFIRM_REJECT_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.INCENTIVES.CONFIRM_REJECT_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  /**
   * @description Prepares and shows the confirmation dialog for deleting an incentive.
   */
  onDelete(): void {
    this.actionToPerform = 'delete'; // تحديد الإجراء كـ 'delete'
    this.translate.get('HR.INCENTIVES.CONFIRM_DELETE_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.INCENTIVES.CONFIRM_DELETE_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  /**
   * @description Performs the approve, reject, or delete action after user confirmation.
   */
  confirmAction(): void {
    this.showConfirmActionDialog = false; // إخفاء مربع التأكيد
    if (!this.incentiveId) {
      console.error('Incentive ID is missing for action.');
      this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
      return;
    }

    let actionObservable: Observable<Result>;
    let successMessageKey: string;
    let errorMessageKey: string;

    switch (this.actionToPerform) {
      case 'approve':
        actionObservable = this.incentiveService.approveRejectIncentive(this.incentiveId, true);
        successMessageKey = 'HR.INCENTIVES.APPROVE_SUCCESS';
        errorMessageKey = 'HR.INCENTIVES.APPROVE_FAILED';
        break;
      case 'reject':
        actionObservable = this.incentiveService.approveRejectIncentive(this.incentiveId, false);
        successMessageKey = 'HR.INCENTIVES.REJECT_SUCCESS';
        errorMessageKey = 'HR.INCENTIVES.REJECT_FAILED';
        break;
      default:
        console.error('Unknown action:', this.actionToPerform);
        this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
        return;
    }

    this.isLoading = true; // إظهار اللودر أثناء تنفيذ العملية

    actionObservable.pipe(
      finalize(() => {
        this.isLoading = false; // إخفاء اللودر بعد انتهاء العملية
      })
    ).subscribe({
      next: response => {
        this.isSuccessAlert = response.isSuccess;
        this.translate.get(response.statusKey || response.statusKey || successMessageKey) // استخدام response.message أو statusKey أولاً
          .subscribe(translatedMessage => {
            this.alertMessage = translatedMessage;
            this.alertVisible = true;
          });
        if (response.isSuccess) {
          if (this.actionToPerform === 'delete') {
            // بعد الحذف، يجب العودة إلى صفحة القائمة
            setTimeout(() => {
              this.router.navigate(['/app/hr/incentives']); // العودة إلى قائمة المكافآت
            }, 1500); // تأخير بسيط للسماح للمستخدم برؤية التنبيه
          } else {
            this.loadIncentiveDetails(this.incentiveId!); // إعادة تحميل البيانات بعد النجاح (للقبول/الرفض)
          }
        }
      },
      error: error => {
        console.error(`Error ${this.actionToPerform}ing incentive:`, error);
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
    this.incentiveSubscription?.unsubscribe();
  }
}
