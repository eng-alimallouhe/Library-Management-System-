// src/app/components/hr/incentives/edit-incentive/edit-incentive.component.ts

import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

// استيراد خدمة المكافآت والـ DTOs الخاصة بها
import { IncentiveService } from '../../../../services/hr/incentive.service';
import { IncentiveDetailsDto } from '../../../../shared/models/hr/incentives/incentive-details.dto';
import { IncentiveUpdateRequestDto } from '../../../../shared/models/hr/incentives/incentive-update-request.dto';

// المكونات المشتركة
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';

@Component({
  selector: 'app-edit-incentive', // تم تغيير السيلكتور
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './update-incentive.component.html', // تم تغيير اسم القالب
  styleUrls: ['./update-incentive.component.css'] // تم تغيير اسم ملف الأنماط
})
export class EditIncentiveComponent implements OnChanges, OnInit { // تم تغيير اسم الكلاس
  updateForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmitted: boolean = false;

  incentiveInformation!: IncentiveDetailsDto; // لتخزين بيانات المكافأة الحالية
  decisionFile: File | undefined; // لتخزين ملف القرار المختار
  currentDecisionFileUrl: string | null = null; // لعرض الرابط الحالي للملف

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() incentiveId: string = ''; // تم تغيير الاسم من penaltyId

  @Output() incentiveUpdated = new EventEmitter<void>(); // تم تغيير الاسم من penaltyUpdated

  constructor(
    private incentiveService: IncentiveService, // استخدام خدمة المكافآت
    private translate: TranslateService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  // استخدام ngOnChanges لمراقبة تغيير الـ Input property (incentiveId أو visible)
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && changes['visible'].currentValue === true && this.incentiveId) {
      this.loadIncentiveData(); // استدعاء دالة جلب البيانات عند فتح المودال
    }
  }

  private initializeForm(): void {
    this.updateForm = this.fb.group({
      employeeName: [{ value: '', disabled: true }], // اسم الموظف للقراءة فقط
      amount: [0, [Validators.required, Validators.min(0)]],
      reason: ['', Validators.required],
    });
  }

  private loadIncentiveData(): void { // تم تغيير الاسم
    this.isLoadingResult = true;
    this.incentiveService.getIncentiveById(this.incentiveId) // استخدام this.incentiveId
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: (response: IncentiveDetailsDto) => {
          this.incentiveInformation = response; // تخزين البيانات للعرض في العنوان مثلاً
          this.updateForm.patchValue({
            employeeName: response.employeeName,
            amount: response.amount,
            reason: response.reason,
          });
          this.currentDecisionFileUrl = response.decisionFileUrl || null;
          this.decisionFile = undefined; // مسح أي ملف تم اختياره مسبقاً
        },
        error: error => {
          console.error('Error loading incentive details for edit:', error);
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || 'COMMON.ERROR_LOADING_DATA')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          this.closeModal();
        }
      });
  }

  // دالة لمعالجة اختيار ملف القرار
  onDecisionFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.decisionFile = input.files[0];
      console.log('Selected decision file:', this.decisionFile.name);
    } else {
      this.decisionFile = undefined;
    }
  }

  onSubmit(): void {
    this.isSubmitted = true;

    if (this.updateForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      return;
    }

    this.isLoadingResult = true;

    const request: IncentiveUpdateRequestDto = {
      amount: this.updateForm.get('amount')?.value,
      reason: this.updateForm.get('reason')?.value,
      decision: this.decisionFile // إضافة الملف المختار
    };

    if (!this.incentiveId) { // استخدام this.incentiveId
      console.error('Incentive ID is missing for update operation.');
      this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
      this.isLoadingResult = false;
      return;
    }

    this.incentiveService.updateIncentive(this.incentiveId, request) // استخدام خدمة المكافآت
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey || 'HR.INCENTIVES.EDIT_SUCCESS') // تحديث مفتاح الترجمة
            .subscribe(translatedMessage => {
              this.showAlertMessage(true, translatedMessage);
            });
          if (response.isSuccess) {
            this.incentiveUpdated.emit(); // إطلاق الحدث لإعادة تحميل البيانات في المكون الأب
          }
        },
        error: error => {
          console.error('Error updating incentive:', error);
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || error.message || 'COMMON.SERVER_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
        }
      });
  }

  // دالة تُستدعى عندما يتم إغلاق التنبيه (بالضغط على زر الإغلاق في الـ Modal Alert)
  onAlertClosed(): void {
    this.alertVisible = false;
    if (this.isSuccessAlert) {
      this.closeModal();
    }
  }

  closeModal(): void {
    this.updateForm.reset();
    this.isSubmitted = false;
    this.visibleChange.emit(false);
  }

  // دوال مساعدة للتحقق من الأخطاء
  getControl(name: string): AbstractControl | null {
    return this.updateForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    return (control?.invalid && (control?.dirty || control?.touched || this.isSubmitted)) ?? false;
  }
  
  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.alertMessage = messageKey;
    this.alertVisible = true;
  }
}
