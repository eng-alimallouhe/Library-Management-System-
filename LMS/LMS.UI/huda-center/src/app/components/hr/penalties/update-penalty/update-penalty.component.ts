import { Component, EventEmitter, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators, AbstractControl } from '@angular/forms'; // إضافة AbstractControl
import { finalize } from 'rxjs/operators';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { PenaltyService } from '../../../../services/hr/penalty.service';
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { PenaltyDetailsDto } from '../../../../shared/models/hr/penalties/penalty-details.dto';
import { PenaltyUpdateRequestDto } from '../../../../shared/models/hr/penalties/penalty-update-request.dto';

@Component({
  selector: 'app-edit-penalty',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    TranslatePipe,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './update-penalty.component.html',
  styleUrls: ['./update-penalty.component.css']
})
export class EditPenaltyComponent implements OnChanges, OnInit {
  updateForm!: FormGroup; // تم تغيير الاسم ليتوافق مع نمط Department
  isLoadingResult: boolean = false; // للتحميل الأولي للبيانات ولإرسال الفورم
  isSubmitted: boolean = false; // لتمكين عرض رسائل الأخطاء بعد الإرسال الأول

  penaltyInformation!: PenaltyDetailsDto; // لتخزين بيانات العقوبة الحالية للعرض (مثل departmentInformation)
  decisionFile: File | undefined; // لتخزين ملف القرار المختار
  currentDecisionFileUrl: string | null = null; // لعرض الرابط الحالي للملف

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false; // تم تغيير الاسم ليتوافق مع نمط Department

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() penaltyId: string = ''; // تم تغيير الاسم ليتوافق مع نمط Department (penaltyIdToEdit -> penaltyId)

  @Output() penaltyUpdated = new EventEmitter<void>(); // تم تغيير الاسم ليتوافق مع نمط Department

  constructor(
    private penaltyService: PenaltyService,
    private translate: TranslateService,
    private fb: FormBuilder
  ) { }

  ngOnInit(): void {
    this.initializeForm();
  }

  // استخدام ngOnChanges لمراقبة تغيير الـ Input property (penaltyId أو visible)
  ngOnChanges(changes: SimpleChanges): void {
    if (changes['visible'] && changes['visible'].currentValue === true && this.penaltyId) {
      this.loadPenaltyData(); // استدعاء دالة جلب البيانات عند فتح المودال
    }
  }

  private initializeForm(): void {
    this.updateForm = this.fb.group({
      employeeName: [{ value: '', disabled: true }], // اسم الموظف للقراءة فقط
      amount: [0, [Validators.required, Validators.min(0)]],
      reason: ['', Validators.required],
      // لم نعد نحتاج decisionDate هنا
    });
  }

  private loadPenaltyData(): void { // تم تغيير الاسم ليتوافق مع نمط Department
    this.isLoadingResult = true; // بدء حالة التحميل
    this.penaltyService.getPenaltyById(this.penaltyId) // استخدام this.penaltyId
      .pipe(
        finalize(() => { this.isLoadingResult = false; }) // إيقاف التحميل بغض النظر عن النتيجة
      )
      .subscribe({
        next: (response: PenaltyDetailsDto) => {
          this.penaltyInformation = response; // تخزين البيانات للعرض في العنوان مثلاً
          this.updateForm.patchValue({
            employeeName: response.employeeName,
            amount: response.amount,
            reason: response.reason,
          });
          this.currentDecisionFileUrl = response.decisionFileUrl || null; // تخزين رابط الملف الحالي
          this.decisionFile = undefined; // مسح أي ملف تم اختياره مسبقاً
        },
        error: error => {
          console.error('Error loading penalty details for edit:', error);
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || 'COMMON.ERROR_LOADING_DATA') // استخدام error.error?.statusKey
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          this.closeModal(); // إغلاق الفورم إذا لم نتمكن من جلب البيانات
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
    this.isSubmitted = true; // لتمكين عرض رسائل الأخطاء في القالب

    if (this.updateForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      return;
    }

    this.isLoadingResult = true; // بدء حالة التحميل للإرسال

    const request: PenaltyUpdateRequestDto = {
      amount: this.updateForm.get('amount')?.value,
      reason: this.updateForm.get('reason')?.value,
      decision: this.decisionFile // إضافة الملف المختار
    };

    if (!this.penaltyId) { // استخدام this.penaltyId
      console.error('Penalty ID is missing for update operation.');
      this.showAlertMessage(false, 'COMMON.ERROR_OCCURRED');
      this.isLoadingResult = false;
      return;
    }

    this.penaltyService.updatePenalty(this.penaltyId, request) // تمرير الـ DTO مباشرة
      .pipe(
        finalize(() => { this.isLoadingResult = false; }) // إيقاف التحميل بغض النظر عن النتيجة
      )
      .subscribe({
        next: response => {
          this.isSuccessAlert = true;
          this.translate.get(response.statusKey || 'HR.PENALTIES.EDIT_SUCCESS') // استخدام response.message أو مفتاح الترجمة
            .subscribe(translatedMessage => {
              this.showAlertMessage(true, translatedMessage);
            });
          if (response.isSuccess) {
            this.penaltyUpdated.emit(); // إطلاق الحدث لإعادة تحميل البيانات في المكون الأب
          }
        },
        error: error => {
          console.error('Error updating penalty:', error);
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || error.message || 'COMMON.SERVER_ERROR') // استخدام error.error?.statusKey
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

  closeModal(): void { // تم تغيير الاسم ليتوافق مع نمط Department
    this.updateForm.reset(); // إعادة تعيين النموذج عند الإغلاق
    this.isSubmitted = false; // إعادة تعيين حالة الإرسال
    this.visibleChange.emit(false); // إغلاق الـ Modal
  }

  

  // دوال مساعدة للتحقق من الأخطاء (مثل DepartmentComponent)
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
