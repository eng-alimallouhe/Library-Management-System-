// src/app/components/hr/penalties/add-penalty/add-penalty.component.ts

import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PenaltyService } from '../../../../services/hr/penalty.service';
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { PenaltyCreateRequestDto } from '../../../../shared/models/hr/penalties/penalty-create-request.dto';

@Component({
  selector: 'app-add-penalty',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './add-penalty.component.html',
  styleUrls: ['./add-penalty.component.css'] // سنستخدم نفس أنماط مودال القسم تقريباً
})
export class AddPenaltyComponent implements OnInit {
  addForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmitted: boolean = false; // لتمكين عرض رسائل الأخطاء بعد الإرسال الأول

  decisionFile: File | undefined; // لتخزين ملف القرار المختار (إجباري هنا)

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() employeeId: string = ''; // معرف الموظف الذي ستضاف له العقوبة

  @Output() penaltyAdded = new EventEmitter<void>(); // لإبلاغ المكون الأب بإضافة عقوبة بنجاح

  constructor(
    private fb: FormBuilder,
    private penaltyService: PenaltyService,
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
    this.isSubmitted = true; // لتمكين عرض رسائل الأخطاء في القالب

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

    const request: PenaltyCreateRequestDto = {
      employeeId: this.employeeId, // تمرير معرف الموظف من الـ Input
      amount: this.addForm.get('amount')?.value,
      reason: this.addForm.get('reason')?.value,
      decisionFile: this.decisionFile // الملف المختار
    };

    this.penaltyService.createPenalty(request)
      .pipe(
        finalize(() => { this.isLoadingResult = false; }) // إيقاف التحميل بغض النظر عن النتيجة
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey || 'HR.PENALTIES.ADD_SUCCESS')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          if (response.isSuccess) {
            this.addForm.reset(); // إعادة تعيين الفورم عند النجاح
            this.isSubmitted = false; // إعادة تعيين حالة الإرسال
            this.decisionFile = undefined; // مسح الملف المختار
            this.penaltyAdded.emit(); // إطلاق الحدث لإبلاغ المكون الأب
          }
        },
        error: error => {
          console.error('Error adding penalty:', error);
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
    this.alertVisible = false; // إخفاء الـ alert
    if (this.isSuccessAlert) { // أغلق الفورم فقط إذا كان التنبيه يخص عملية ناجحة
      this.closeModal(); // استدعاء دالة إغلاق الفورم
    }
  }

  closeModal(): void { // دالة إغلاق المودال
    this.addForm.reset();
    this.isSubmitted = false;
    this.decisionFile = undefined; // مسح الملف المختار عند الإغلاق
    this.visibleChange.emit(false); // إغلاق الـ Modal
  }

  // دوال مساعدة للتحقق من الأخطاء
  getControl(name: string): AbstractControl | null {
    return this.addForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    return (control?.invalid && (control?.dirty || control?.touched || this.isSubmitted)) ?? false;
  }

  // دالة مساعدة لعرض رسالة التنبيه (مثل EditPenaltyComponent)
  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.alertMessage = messageKey;
    this.alertVisible = true;
  }
}
