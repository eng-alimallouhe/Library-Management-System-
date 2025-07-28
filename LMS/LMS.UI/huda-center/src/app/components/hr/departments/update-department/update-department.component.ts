// src/app/components/hr/departments/update-department/update-department.component.ts

import { Component, EventEmitter, Input, Output, OnInit, OnChanges, SimpleChanges } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs';

// استيراد الخدمات والـ DTOs
import { DepartmentService } from '../../../../services/hr/department.service';
import { DepartmentRequestDto } from '../../../../shared/models/hr/departments/department-request.dto';
import { DepartmenttoUpdateDto } from '../../../../shared/models/hr/departments/department-to-update.dto'; // تأكد من المسار الصحيح

// المكونات المشتركة
import { LoaderComponent } from "../../../../shared/components/loader/loader.component";
import { AlterComponent } from "../../../../shared/components/alter/alter.component";

@Component({
  selector: 'app-update-department',
  standalone: true,
  imports: [
    TranslatePipe,
    CommonModule,
    ReactiveFormsModule,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './update-department.component.html',
  styleUrl: './update-department.component.css'
})
export class UpdateDepartmentComponent implements OnInit, OnChanges {
  updateForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmited: boolean = false;
  
  departmentInformation!: DepartmenttoUpdateDto; // لتخزين بيانات القسم الحالية للعرض

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() departmentId: string = ''; // ID القسم الذي سيتم تعديله

  @Output() departmentUpdated = new EventEmitter<void>(); // لإخبار المكون الأب بتحديث البيانات

  constructor(
    private departmentService: DepartmentService,
    private translate: TranslateService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.initializeForm();
  }

  // استخدام ngOnChanges لمراقبة تغيير الـ Input property (departmentId أو visible)
  // هذا يضمن أن يتم جلب البيانات كلما تم فتح الـ modal لقسم جديد
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && changes['visible'].currentValue === true && this.departmentId) {
      this.loadDepartmentData();
    }
  }

  private initializeForm() {
    this.updateForm = this.fb.group({
      departmentName: ['', [
        Validators.required,
        Validators.maxLength(100)
      ]],
      departmentDescription: ['', [
        Validators.maxLength(500)
      ]]
    });
  }

  private loadDepartmentData(): void {
    this.isLoadingResult = true; // استخدام نفس متغير التحميل للتحكم باللودر
    this.departmentService.getDepartmentSnapshot(this.departmentId)
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: response => {
          this.departmentInformation = response; // تخزين البيانات للعرض في العنوان مثلاً
          this.updateForm.patchValue({
            departmentName: response.departmentName,
            departmentDescription: response.departmentDescription
          });
        },
        error: error => {
          console.error('Error loading department snapshot:', error);
          // يمكنك عرض تنبيه هنا إذا فشل جلب البيانات
          this.isSuccessAlert = false;
          this.translate.get(error.error.statusKey || 'UNKNOWN_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          this.close(); // إغلاق الفورم إذا لم نتمكن من جلب البيانات
        }
      });
  }

  onSubmit() {
    this.isSubmited = true;

    if (this.updateForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      return;
    }

    const request: DepartmentRequestDto = {
      departmentName: this.updateForm.value.departmentName,
      departmentDescription: this.updateForm.value.departmentDescription || ''
    };

    this.isLoadingResult = true;

    this.departmentService.updateDepartment(this.departmentId, request)
      .pipe(
        finalize(() => { this.isLoadingResult = false; })
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey)
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          if (response.isSuccess) {
            // لا نغلق الفورم هنا مباشرة، ننتظر إغلاق الـ Alert
            this.departmentUpdated.emit(); // إطلاق الحدث لإعادة تحميل البيانات في المكون الأب
          }
        },
        error: error => {
          this.isSuccessAlert = false;
          this.translate.get(error.error.statusKey || 'UNKNOWN_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
        }
      });
  }

  // دالة تُستدعى عندما يتم إغلاق التنبيه (بالضغط على زر الإغلاق في الـ Modal Alert)
  onAlertClosed(): void {
    if (this.isSuccessAlert) { // أغلق الفورم فقط إذا كان التنبيه يخص عملية ناجحة
      this.close(); // استدعاء دالة إغلاق الفورم
    }
  }

  close() {
    this.updateForm.reset(); // إعادة تعيين النموذج عند الإغلاق
    this.isSubmited = false; // إعادة تعيين حالة الإرسال
    this.visibleChange.emit(false); // إغلاق الـ Modal
  }

  getControl(name: string) {
    return this.updateForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    return (control?.invalid && (control?.dirty || control?.touched || this.isSubmited)) ?? false;
  }
}
