// src/app/components/hr/employee/employee.component.ts

import { Component, OnInit } from '@angular/core';
import { finalize, map, Observable } from 'rxjs';
import { EmployeeDetailsDto } from '../../../../shared/models/hr/employee/employee-details.dto';
import { AsyncPipe, CommonModule } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core'; // إضافة TranslateService
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { EmployeeService } from '../../../../services/hr/employee.service';
import { LocalDatePipe } from "../../../../pipes/local-date.pipe";
import { LoaderComponent } from "../../../../shared/components/loader/loader.component";
import { UpdateEmployeeComponent } from "../update-employee/update-employee.component";
import { ConfirmDialogComponent } from "../../../../shared/components/confirm-dialog/confirm-dialog.component";
import { AddPenaltyComponent } from '../../penalties/add-penalty/add-penalty.component'; // استيراد مكون إضافة العقوبة
import { AlterComponent } from '../../../../shared/components/alter/alter.component';
import { AddIncentiveComponent } from "../../incentives/add-incentive/add-incentive.component"; // استيراد مكون التنبيه

@Component({
  selector: 'app-employee',
  standalone: true, // تأكد أنها standalone
  imports: [
    CommonModule,
    AsyncPipe,
    TranslatePipe,
    LocalDatePipe,
    LoaderComponent,
    RouterLink,
    UpdateEmployeeComponent,
    ConfirmDialogComponent,
    AddPenaltyComponent, // تضمين مكون إضافة العقوبة
    AlterComponent // تضمين AlterComponent
    ,
    AddIncentiveComponent
],
  templateUrl: './employee.component.html',
  styleUrl: './employee.component.css'
})
export class EmployeeComponent implements OnInit { // تطبيق OnInit
  employeeId: string = '';
  employee$!: Observable<EmployeeDetailsDto>;
  isLoadingEmployee: boolean = true;

  showConfirm = false; // لحذف الموظف

  isEmployeeUpdateFromVisible = false; // لتعديل الموظف
  isAddPenaltyFormVisible = false; // لإضافة عقوبة جديدة (جديد)
  isAddIncentiveFormVisible = false;

  // متغيرات Alert
  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;

  constructor(
    private employeeService: EmployeeService,
    private route: ActivatedRoute,
    private router: Router,
    private translate: TranslateService // حقن TranslateService
  ) { }

  ngOnInit() {
    this.isLoadingEmployee = true;
    this.employeeId = this.route.snapshot.paramMap.get('id') || ' ';
    console.log(this.employeeId);

    // استدعاء دالة لتحميل التفاصيل عند تهيئة المكون
    this.loadEmployeeDetails();
  }

  private loadEmployeeDetails(): void {
    // تأكد من أن employeeId ليس فارغاً قبل محاولة جلب البيانات
    if (!this.employeeId || this.employeeId === ' ') {
      console.error('Employee ID is missing in the URL.');
      this.isLoadingEmployee = false;
      this.showAlertMessage(false, 'COMMON.ERROR_LOADING_DATA'); // عرض رسالة خطأ
      return;
    }

    this.isLoadingEmployee = true;
    this.employee$ = this.employeeService.getEmployee(this.employeeId).pipe(
      map(employee => ({
        ...employee,
        departmentsHistory: employee.departmentsHistory?.slice().sort((a, b) =>
          new Date(b.joinDate).getTime() - new Date(a.joinDate).getTime()
        ),
        employeeFinanical: employee.employeeFinanical?.slice().sort((a, b) =>
          new Date(b.date).getTime() - new Date(a.date).getTime()
        ),
        attendances: employee.attendances?.slice().sort((a, b) =>
          new Date(b.date).getTime() - new Date(a.date).getTime()
        ),
        // إضافة ترتيب العقوبات هنا
        penalties: employee.penalties?.slice().sort((a, b) =>
          new Date(b.decisionDate).getTime() - new Date(a.decisionDate).getTime()
        ),
        // إضافة ترتيب الحوافز هنا
        incentives: employee.incentives?.slice().sort((a, b) =>
          new Date(b.decisionDate).getTime() - new Date(a.decisionDate).getTime()
        ),
        // إضافة ترتيب الرواتب هنا
        salaries: employee.salaries?.slice().sort((a, b) => {
          // ترتيب الرواتب حسب السنة ثم الشهر
          if (b.year !== a.year) {
            return b.year - a.year;
          }
          return b.month - a.month;
        }),
        // إضافة ترتيب الإجازات هنا
        leaves: employee.leaves?.slice().sort((a, b) =>
          new Date(b.startDate).getTime() - new Date(a.startDate).getTime()
        )
      })),
      finalize(() => {
        this.isLoadingEmployee = false;
      })
    );
  }

  // تم تعديل onEdit لإزالة البارامتر حيث أن employeeId هو خاصية في الكلاس
  onEdit(): void {
    this.isEmployeeUpdateFromVisible = true;
  }

  onDelete(): void {
    this.showConfirm = true;
  }

  deleteEmployee(): void {
    this.employeeService.deleteEmployee(this.employeeId)
      .subscribe({
        next: response => {
          this.showConfirm = false;
          this.isSuccessAlert = response.isSuccess;
          this.translate.get(response.statusKey || 'COMMON.SUCCESS')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
          if (response.isSuccess) {
            // توجيه بعد فترة قصيرة للسماح للمستخدم برؤية التنبيه
            setTimeout(() => {
              this.router.navigate(['/app/employees']);
            }, 3000);
          }
        },
        error: error => {
          this.showConfirm = false;
          this.isSuccessAlert = false;
          this.translate.get(error.error?.statusKey || 'COMMON.ERROR_OCCURRED')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true;
            });
        }
      });
  }

  onCancelDelete(): void {
    this.showConfirm = false;
  }

  // دالة لفتح مودال إضافة العقوبة
  onAddPenalty(): void {
    this.isAddPenaltyFormVisible = true;
  }

  // دالة يتم استدعاؤها عند إضافة عقوبة بنجاح
  onPenaltyAdded(): void {
    console.log('Penalty added successfully. Reloading employee details...');
    this.loadEmployeeDetails(); // أعد تحميل بيانات الموظف لتحديث قائمة العقوبات
    // لا داعي لإخفاء المودال هنا، لأن مكون الإضافة نفسه سيقوم بإخفائه عند النجاح
  }


  onAddIncentive(): void {
    this.isAddIncentiveFormVisible = true;
  }

  // جديد: دالة يتم استدعاؤها عند إضافة مكافأة بنجاح
  onIncentiveAdded(): void {
    console.log('Incentive added successfully. Reloading employee details...');
    this.loadEmployeeDetails();
  }

  // دالة لمعالجة إغلاق Alert
  onAlertClosed(): void {
    this.alertVisible = false;
    // يمكن إضافة منطق هنا إذا كنت تريد فعل شيء بعد إغلاق التنبيه
  }

  // دالة مساعدة لعرض رسالة التنبيه
  private showAlertMessage(isSuccess: boolean, messageKey: string): void {
    this.isSuccessAlert = isSuccess;
    this.translate.get(messageKey).subscribe(translatedMessage => {
      this.alertMessage = translatedMessage;
      this.alertVisible = true;
    });
  }
}
