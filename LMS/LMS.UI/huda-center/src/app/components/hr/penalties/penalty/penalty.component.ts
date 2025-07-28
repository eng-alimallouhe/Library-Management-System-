// src/app/components/hr/penalties/penalty-details/penalty-details.component.ts

import { Component, OnInit, OnDestroy } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core'; // إضافة TranslateService
import { finalize } from 'rxjs/operators';
import { Subscription } from 'rxjs';

import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { PenaltyService } from '../../../../services/hr/penalty.service';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { PenaltyDetailsDto } from '../../../../shared/models/hr/penalties/penalty-details.dto';

import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component'; // استيراد مكون التأكيد
import { AlterComponent } from '../../../../shared/components/alter/alter.component'; // استيراد مكون التنبيه
import { EditPenaltyComponent } from '../update-penalty/update-penalty.component';

@Component({
  selector: 'app-penalty-details',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    LoaderComponent,
    LocalDatePipe,
    EditPenaltyComponent,
    ConfirmDialogComponent, // تضمين مكون التأكيد
    AlterComponent // تضمين مكون التنبيه
  ],
  templateUrl: './penalty.component.html',
  styleUrls: ['./penalty.component.css']
})
export class PenaltyComponent implements OnInit, OnDestroy {
  penaltyId: string | null = null;
  penaltyDetails: PenaltyDetailsDto | null = null;
  isLoading = true;
  private routeSubscription: Subscription | undefined;
  private penaltySubscription: Subscription | undefined;

  isPenaltyUpdateFormVisible = false;

  // متغيرات Confirm Dialog
  showConfirmActionDialog: boolean = false;
  confirmDialogTitle: string = '';
  confirmDialogMessage: string = '';
  private actionToPerform: 'approve' | 'reject' | null = null; // لتخزين نوع الإجراء

  // متغيرات Alert
  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    private penaltyService: PenaltyService,
    private sanitizer: DomSanitizer,
    private translate: TranslateService // حقن TranslateService
  ) { }

  ngOnInit(): void {
    console.log('ngOnInit: Component initialized.');
    this.routeSubscription = this.route.paramMap.subscribe(params => {
      this.penaltyId = params.get('id');
      console.log('ngOnInit: Penalty ID from route:', this.penaltyId);
      if (this.penaltyId) {
        this.loadPenaltyDetails(this.penaltyId);
      } else {
        console.error('Penalty ID not found in route parameters.');
        this.isLoading = false;
      }
    });
  }

  private loadPenaltyDetails(id: string): void {
    this.isLoading = true;
    console.log('loadPenaltyDetails: Fetching penalty details for ID:', id);
    this.penaltySubscription = this.penaltyService.getPenaltyById(id)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          console.log('loadPenaltyDetails: Loading finished.');
        })
      )
      .subscribe({
        next: (data) => {
          this.penaltyDetails = data;
          console.log('loadPenaltyDetails: Penalty details received:', this.penaltyDetails);
          if (!this.penaltyDetails.decisionFileUrl) {
            console.log('loadPenaltyDetails: No decision file URL found.');
          }
        },
        error: (err) => {
          console.error('Error loading penalty details:', err);
          this.penaltyDetails = null;
          // عرض رسالة خطأ إذا فشل تحميل التفاصيل
          this.isSuccessAlert = false;
          this.translate.get('COMMON.ERROR_LOADING_DATA')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
        }
      });
  }

  get downloadLinkSafeUrl(): SafeResourceUrl {
    return this.sanitizer.bypassSecurityTrustResourceUrl(this.penaltyDetails?.decisionFileUrl || '');
  }

  onEdit(): void {
    this.isPenaltyUpdateFormVisible = true;
    console.log('Edit penalty form visibility set to:', this.isPenaltyUpdateFormVisible);
  }

  onPenaltyUpdated(): void {
    console.log('Penalty updated successfully, reloading details...');
    if (this.penaltyId) {
      this.loadPenaltyDetails(this.penaltyId);
    }
  }

  /**
   * @description Prepares and shows the confirmation dialog for approving a penalty.
   */
  onApprove(): void {
    this.actionToPerform = 'approve';
    this.translate.get('HR.PENALTIES.CONFIRM_APPROVE_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.PENALTIES.CONFIRM_APPROVE_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  /**
   * @description Prepares and shows the confirmation dialog for rejecting a penalty.
   */
  onReject(): void {
    this.actionToPerform = 'reject';
    this.translate.get('HR.PENALTIES.CONFIRM_REJECT_TITLE').subscribe(title => {
      this.confirmDialogTitle = title;
    });
    this.translate.get('HR.PENALTIES.CONFIRM_REJECT_MESSAGE').subscribe(message => {
      this.confirmDialogMessage = message;
    });
    this.showConfirmActionDialog = true;
  }

  /**
   * @description Performs the approve or reject action after user confirmation.
   */
  confirmPenaltyAction(): void {
    this.showConfirmActionDialog = false; // إخفاء مربع التأكيد
    if (!this.penaltyId) {
      console.error('Penalty ID is missing for approve/reject action.');
      this.isSuccessAlert = false;
      this.translate.get('COMMON.ERROR_OCCURRED').subscribe(translatedMessage => {
        this.alertMessage = translatedMessage;
        this.alertVisible = true;
      });
      return;
    }

    let isApproved = this.actionToPerform === 'approve';
    let successMessageKey = isApproved ? 'HR.PENALTIES.APPROVE_SUCCESS' : 'HR.PENALTIES.REJECT_SUCCESS';
    let errorMessageKey = isApproved ? 'HR.PENALTIES.APPROVE_FAILED' : 'HR.PENALTIES.REJECT_FAILED';

    this.penaltyService.approvePenalty(this.penaltyId, isApproved)
      .pipe(
        finalize(() => {
        })
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey || successMessageKey)
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          if (response.isSuccess) {
            this.loadPenaltyDetails(this.penaltyId!); // إعادة تحميل البيانات بعد النجاح
          }
        },
        error: error => {
          console.error(`Error ${this.actionToPerform}ing penalty:`, error);
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

  ngOnDestroy(): void {
    this.routeSubscription?.unsubscribe();
    this.penaltySubscription?.unsubscribe();
  }
}
