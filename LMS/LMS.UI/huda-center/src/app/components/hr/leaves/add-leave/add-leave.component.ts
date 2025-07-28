// src/app/components/hr/leaves/add-leave/add-leave.component.ts

import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule, AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs';

// استيراد خدمة الإجازات والـ DTOs الخاصة بها
import { LeaveService } from '../../../../services/hr/leave.service';
import { LeaveType } from '../../../../shared/models/hr/leaves/leave-type.enum'; // استيراد enum أنواع الإجازات

// المكونات المشتركة
import { LoaderComponent } from "../../../../shared/components/loader/loader.component";
import { AlterComponent } from "../../../../shared/components/alter/alter.component";
import { LeaveCreateRequestDto } from '../../../../shared/models/hr/leaves/leaves-create-request.dto.ts';

@Component({
  selector: 'app-add-leave',
  standalone: true,
  imports: [
    TranslatePipe,
    CommonModule,
    ReactiveFormsModule,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './add-leave.component.html',
  styleUrl: './add-leave.component.css' // تأكد من أن هذا المسار صحيح
})
export class AddLeaveComponent implements OnInit {
  addForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmitted: boolean = false; // تم تصحيح الاسم من isSubmited

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  @Input() employeeId!: string; // <--- معرف الموظف الذي سيقدم الإجازة له
  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Output() leaveAdded = new EventEmitter<void>(); // حدث يتم إطلاقه عند إضافة إجازة بنجاح

  // خيارات أنواع الإجازات للعرض في الـ select
  leaveTypeOptions = [
    { value: LeaveType.Annual, label: 'HR.LEAVES.LEAVES.TYPE.ANNUAL' },
    { value: LeaveType.Sick, label: 'HR.LEAVES.LEAVES.TYPE.SICK' },
    { value: LeaveType.Unpaid, label: 'HR.LEAVES.LEAVES.TYPE.UNPAID' },
    { value: LeaveType.Maternity, label: 'HR.LEAVES.LEAVES.TYPE.MATERNITY' },
    { value: LeaveType.Paternity, label: 'HR.LEAVES.LEAVES.TYPE.PATERNITY' },
    { value: LeaveType.Marriage, label: 'HR.LEAVES.LEAVES.TYPE.MARRIAGE' },
    { value: LeaveType.Death, label: 'HR.LEAVES.LEAVES.TYPE.DEATH' },
    { value: LeaveType.Study, label: 'HR.LEAVES.LEAVES.TYPE.STUDY' },
    { value: LeaveType.Hajj, label: 'HR.LEAVES.LEAVES.TYPE.HAJJ' },
    { value: LeaveType.OfficialLeave, label: 'HR.LEAVES.LEAVES.TYPE.OFFICIALLEAVE' },
    { value: LeaveType.Other, label: 'HR.LEAVES.LEAVES.TYPE.OTHER' },
  ];

  constructor(
    private leaveService: LeaveService,
    private translate: TranslateService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.initializeForm();
  }

  private initializeForm(): void {
    this.addForm = this.fb.group({
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      leaveType: [null, Validators.required], // قيمة أولية null لـ select
      reason: ['', Validators.required],
    }, {
      validators: this.dateRangeValidator() // تطبيق الـ validator على مستوى الـ FormGroup
    });
  }

  // نفس الـ validator الذي تم إصلاحه في مكون التعديل
  private dateRangeValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const startDateControl = control.get('startDate');
      const endDateControl = control.get('endDate');

      if (startDateControl && endDateControl && startDateControl.value && endDateControl.value) {
        const startDate = new Date(startDateControl.value);
        const endDate = new Date(endDateControl.value);

        // إذا كان تاريخ البدء أكبر من تاريخ الانتهاء، أعد الخطأ
        if (startDate > endDate) {
          return { 'dateRangeInvalid': true };
        }
      }
      return null;
    };
  }

  onSubmit() {
    this.isSubmitted = true;
    this.addForm.updateValueAndValidity(); // تحديث حالة الـ validator يدوياً

    if (this.addForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      if (this.addForm.errors?.['dateRangeInvalid']) {
        this.showAlertMessage(false, 'VALIDATION.DATE_RANGE_INVALID');
      }
      return;
    }

    const request: LeaveCreateRequestDto = {
      employeeId: this.employeeId, // <--- تمرير معرف الموظف
      startDate: this.addForm.value.startDate,
      endDate: this.addForm.value.endDate,
      leaveType: this.addForm.value.leaveType,
      reason: this.addForm.value.reason,
    };

    this.isLoadingResult = true;
    console.log('Submitting leave creation request...');

    this.leaveService.createLeave(request) // <--- استخدام خدمة إضافة الإجازة
      .pipe(
        finalize(() => {
          this.isLoadingResult = false;
          console.log('API call finalized. isLoadingResult set to false.');
        })
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          console.log('API Response received. isSuccessAlert:', this.isSuccessAlert, 'Status Key:', response.statusKey);

          this.translate.get(response.statusKey || 'HR.LEAVES.ADD_SUCCESS') // مفتاح ترجمة للنجاح
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
              console.log('Alert message set. alertVisible set to true.');
            });
          if (response.isSuccess) {
            this.addForm.reset();
            this.isSubmitted = false;
            this.leaveAdded.emit(); // إطلاق الحدث لإعادة تحميل البيانات في المكون الأب
            console.log('Leave added successfully. leaveAdded event emitted.');
          }
        },
        error: error => {
          this.isSuccessAlert = false;
          console.error('API Error:', error);
          this.translate.get(error.error?.statusKey || error.message || 'COMMON.SERVER_ERROR') // مفتاح ترجمة للخطأ
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
              console.log('Error alert message set. alertVisible set to true.');
            });
        }
      });
  }

  onAlertClosed(): void {
    this.alertVisible = false;
    if (this.isSuccessAlert) {
      this.closeModal(); // إغلاق المودال عند النجاح
    }
  }

  closeModal() { // تم تغيير الاسم من close إلى closeModal للتناسق
    this.addForm.reset();
    this.isSubmitted = false;
    this.visibleChange.emit(false);
  }

  getControl(name: string): AbstractControl | null { // إضافة نوع الإرجاع
    return this.addForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    // إذا كان الخطأ على مستوى عنصر التحكم نفسه
    if (control?.invalid && (control?.dirty || control?.touched || this.isSubmitted)) {
      return true;
    }
    // إذا كان الخطأ على مستوى النموذج ككل (مثل dateRangeInvalid)
    // وتأكدنا أن هذا عنصر التحكم هو endDate لعرض الخطأ تحته
    if (controlName === 'endDate' && this.addForm.errors?.['dateRangeInvalid'] && (this.addForm.dirty || this.addForm.touched || this.isSubmitted)) {
      return true;
    }
    return false;
  }

  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.alertMessage = messageKey;
    this.alertVisible = true;
  }
}
