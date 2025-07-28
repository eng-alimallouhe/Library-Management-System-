// src/app/components/hr/incentives/add-incentive/add-incentive.component.ts

import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

// استيراد خدمة المكافآت والـ DTOs الخاصة بها
import { IncentiveService } from '../../../../services/hr/incentive.service';
import { IncentiveCreateRequestDto } from '../../../../shared/models/hr/incentives/incentive-create-request.dto';

// المكونات المشتركة
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';

@Component({
  selector: 'app-add-incentive', // تم تغيير السيلكتور
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './add-incentive.component.html', // تم تغيير اسم القالب
  styleUrls: ['./add-incentive.component.css'] // تم تغيير اسم ملف الأنماط
})
export class AddIncentiveComponent implements OnInit { // تم تغيير اسم الكلاس
  addForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmitted: boolean = false;

  decisionFile: File | undefined; // لتخزين ملف القرار المختار (إجباري هنا)

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() employeeId: string = ''; // معرف الموظف الذي ستضاف له المكافأة

  @Output() incentiveAdded = new EventEmitter<void>(); // لإبلاغ المكون الأب بإضافة مكافأة بنجاح

  constructor(
    private fb: FormBuilder,
    private incentiveService: IncentiveService, // استخدام خدمة المكافآت
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.addForm = this.fb.group({
      amount: [0, [Validators.required, Validators.min(0)]],
      reason: ['', Validators.required],
      // لا يوجد formControl لـ decisionFile لأنه يتم التعامل معه كـ File منفصل
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

    // التحقق من صحة الفورم ووجود الملف
    if (this.addForm.invalid) {
      console.log('Form is invalid.');
      this.showAlertMessage(false, 'COMMON.FORM_INVALID_FIELDS');
      return;
    }
    if (!this.decisionFile) {
      console.log('Decision file is missing.');
      this.showAlertMessage(false, 'VALIDATION.DECISION_FILE_REQUIRED'); // رسالة خطأ خاصة بالملف
      return;
    }

    this.isLoadingResult = true; // بدء حالة التحميل للإرسال

    const request: IncentiveCreateRequestDto = { // استخدام IncentiveCreateRequestDto
      employeeId: this.employeeId, // تمرير معرف الموظف من الـ Input
      amount: this.addForm.get('amount')?.value,
      reason: this.addForm.get('reason')?.value,
      decisionFile: this.decisionFile // الملف المختار
    };

    this.incentiveService.createIncentive(request) // استخدام خدمة المكافآت
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: (response) => { // التأكد من نوع الاستجابة
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey || 'HR.INCENTIVES.ADD_SUCCESS') // تحديث مفتاح الترجمة
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          if (response.isSuccess) {
            this.addForm.reset();
            this.isSubmitted = false;
            this.decisionFile = undefined;
            this.incentiveAdded.emit(); // إطلاق الحدث لإبلاغ المكون الأب
          }
        },
        error: error => {
          console.error('Error adding incentive:', error);
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || error.message || 'COMMON.SERVER_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
        }
      });
  }

  // دالة تُستدعى عندما يتم إغلاق التنبيه
  onAlertClosed(): void {
    this.alertVisible = false;
    if (this.isSuccessAlert) {
      this.closeModal();
    }
  }

  closeModal(): void { // دالة إغلاق المودال
    this.addForm.reset();
    this.isSubmitted = false;
    this.decisionFile = undefined;
    this.visibleChange.emit(false);
  }

  // دوال مساعدة للتحقق من الأخطاء
  getControl(name: string): AbstractControl | null {
    return this.addForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    return (control?.invalid && (control?.dirty || control?.touched || this.isSubmitted)) ?? false;
  }

  // دالة مساعدة لعرض رسالة التنبيه
  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.alertMessage = messageKey;
    this.alertVisible = true;
  }
}
