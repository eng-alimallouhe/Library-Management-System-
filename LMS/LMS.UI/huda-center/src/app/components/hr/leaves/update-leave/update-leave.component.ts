// src/app/components/hr/leaves/edit-leave/edit-leave.component.ts

import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl, ValidatorFn, ValidationErrors } from '@angular/forms';
import { finalize } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

// استيراد خدمة الإجازات والـ DTOs الخاصة بها
import { LeaveService } from '../../../../services/hr/leave.service';
import { LeaveDetailsDto } from '../../../../shared/models/hr/leaves/leave-details.dto';
import { LeaveType } from '../../../../shared/models/hr/leaves/leave-type.enum';
import { LeaveUpdateRequestDto } from '../../../../shared/models/hr/leaves/leave-update-request.dto';

// المكونات المشتركة
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';

@Component({
  selector: 'app-edit-leave',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './update-leave.component.html',
  styleUrls: ['./update-leave.component.css']
})
export class EditLeaveComponent implements OnChanges, OnInit {
  updateForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmitted: boolean = false;

  leaveInformation!: LeaveDetailsDto;

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() leaveId: string = '';

  @Output() leaveUpdated = new EventEmitter<void>();

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
  ) {
    this.initializeForm();
  }

  ngOnInit(): void {
    // ngOnInit لا يحتاج لاستدعاء initializeForm() أو loadLeaveData()
  }

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && changes['visible'].currentValue === true && this.leaveId) {
      this.loadLeaveData();
    }
  }

  private initializeForm(): void {
    this.updateForm = this.fb.group({
      employeeName: [{ value: '', disabled: true }],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
      leaveType: [null, Validators.required],
      reason: ['', Validators.required],
    }, {
      validators: this.dateRangeValidator
    });
  }

  private dateRangeValidator(control: AbstractControl): ValidationErrors | null {
    const startDate = control.get('startDate');
    const endDate = control.get('endDate');

    if (startDate?.value && endDate?.value) {
      if (startDate.value > endDate.value) {
        endDate.setErrors({ dateRangeInvalid: true })
      }
      else{
        if (endDate.hasError('dateRangeInvalid')) {
          const errors = { ...endDate.errors };
          delete errors['dateRangeInvalid'];
          const hasOtherErrors = Object.keys(errors).length > 0;
          endDate.setErrors(hasOtherErrors ? errors : null);
        }
      }
    }
    return null;
  }

  private loadLeaveData(): void {
    this.isLoadingResult = true;
    this.leaveService.getLeaveById(this.leaveId)
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: (response: LeaveDetailsDto) => {
          this.leaveInformation = response;

          // <--- التغيير هنا: تحويل تاريخ الـ UTC إلى تاريخ محلي لملء حقول النموذج
          const dateObjStart = new Date(response.startDate);
          const dateObjEnd = new Date(response.endDate);

          // الحصول على مكونات التاريخ المحلية (السنة، الشهر، اليوم)
          const localStartDate = `${dateObjStart.getFullYear()}-${(dateObjStart.getMonth() + 1).toString().padStart(2, '0')}-${dateObjStart.getDate().toString().padStart(2, '0')}`;
          const localEndDate = `${dateObjEnd.getFullYear()}-${(dateObjEnd.getMonth() + 1).toString().padStart(2, '0')}-${dateObjEnd.getDate().toString().padStart(2, '0')}`;
          // ---> نهاية التغيير

          this.updateForm.patchValue({
            employeeName: response.employeeName,
            startDate: localStartDate, // استخدام التاريخ المحلي
            endDate: localEndDate,     // استخدام التاريخ المحلي
            leaveType: response.leaveType,
            reason: response.reason,
          });
        },
        error: error => {
          console.error('Error loading leave details for edit:', error);
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

  onSubmit(): void {
    this.isSubmitted = true;

    if (this.updateForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      if (this.updateForm.errors?.['dateRangeInvalid']) {
        this.showAlertMessage(false, 'VALIDATION.DATE_RANGE_INVALID');
      }
      return;
    }

    this.isLoadingResult = true;

    const request: LeaveUpdateRequestDto = {
      employeeId: this.leaveInformation.employeeId,
      startDate: this.updateForm.get('startDate')?.value,
      endDate: this.updateForm.get('endDate')?.value,
      leaveType: this.updateForm.get('leaveType')?.value,
      reason: this.updateForm.get('reason')?.value,
    };

    if (!this.leaveId) {
      console.error('Leave ID is missing for update operation.');
      this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
      this.isLoadingResult = false;
      return;
    }

    this.leaveService.updateLeave(this.leaveId, request)
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey || 'HR.LEAVES.EDIT_SUCCESS')
            .subscribe(translatedMessage => {
              this.showAlertMessage(true, translatedMessage);
            });
          if (response.isSuccess) {
            this.leaveUpdated.emit();
          }
        },
        error: error => {
          console.error('Error updating leave:', error);
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || error.message || 'COMMON.SERVER_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
        }
      });
  }

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
