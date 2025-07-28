import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize, Observable, takeUntil } from 'rxjs';
import { AsyncPipe, CommonModule, JsonPipe } from '@angular/common';
import { LanguageService } from '../../../../shared/services/language.service';
import { EmployeeService } from '../../../../services/hr/employee.service';
import { DepartmentService } from '../../../../services/hr/department.service';
import { AvaliableDepartmentDto } from '../../../../shared/models/hr/departments/avaliable-departments.dto';
import { EmployeeAddingResponseDto } from '../../../../shared/models/hr/employee/employee-adding-response.dto';
import { EmployeeCreateRequestDto } from '../../../../shared/models/hr/employee/employee-create-request.dto';
import { LoaderComponent } from "../../../../shared/components/loader/loader.component";
import { EmployeeCredentialDialogComponent } from "../employee-credential-dialog/employee-credential-dialog.component";
import { AlterComponent } from "../../../../shared/components/alter/alter.component";

@Component({
  selector: 'app-add-employee',
  imports: [
    TranslatePipe,
    AsyncPipe,
    CommonModule,
    ReactiveFormsModule,
    LoaderComponent,
    EmployeeCredentialDialogComponent,
    AlterComponent
],
  templateUrl: './add-employee.component.html',
  styleUrl: './add-employee.component.css'
})
export class AddEmployeeComponent {
  addForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmited: boolean = false;
  responseData: EmployeeAddingResponseDto = {email: '', password: '', userName: ''};
  departments$!: Observable<AvaliableDepartmentDto[]>;
  appointmentDecisionFile!: File;
  faceImageFile!: File;
  isLoadingDepartments = true;
  isDialogShown : boolean = false;

  alertVisible = false;
  alertMessage = '';

  employeeInfo = {
    useName: 'test',
    email: 'test@gmail.com',
    password: 'testpassword'
  }

  isResultLoaded = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;


  constructor(
    private employeeService: EmployeeService,
    private departmentService: DepartmentService,
    private translate: TranslateService,
    private fb: FormBuilder,
    private router: Router,
    private languageService: LanguageService
  ) { }


  ngOnInit() {
    this.departments$ = this.departmentService
    .getAvaliableDepartments(null)
    .pipe(finalize(() => {
      this.isLoadingDepartments = false;
    }));

    this.addForm = this.fb.group({
      fullName: ['', [
        Validators.required,
        Validators.maxLength(100)
      ]],

      email: ['', [
        Validators.required,
        Validators.maxLength(256)
      ]],
      
      phoneNumber: ['', [
        Validators.required,
        Validators.maxLength(256)
      ]],
      
      departmentId: ['', [
        Validators.required
      ]],
      baseSalary: ['', [
        Validators.required
      ]]
    });
  }


  onSubmit(){
    const request : EmployeeCreateRequestDto = {
      fullName: this.addForm.value.fullName,
      email: this.addForm.value.email,
      phoneNumber: this.addForm.value.phoneNumber,
      departmentId: this.addForm.value.departmentId,
      language: this.languageService.getLanguageAsNumber(),
      appointmentDecision: this.appointmentDecisionFile,
      FaceImage: this.faceImageFile,
      baseSalary: this.addForm.value.baseSalary
    };
    
    if (this.addForm.invalid || !this.faceImageFile || !this.appointmentDecisionFile) {
      this.isSubmited = true;
      return;
    }

  
    this.isSubmited = false;


    this.isLoadingResult = true;

    this.employeeService.addEmployee(request)
    .pipe(
      finalize( () => {this.isLoadingResult = false}))
    .subscribe({
      next: response => {
        this.showResultInfroamtion(response.value);
      },
      error: error => {
        this.translate.get(error.error.statusKey)
        .subscribe(translatedMessage => {
          this.alertMessage = translatedMessage;
          this.alertVisible = true;
        });
      } 
    })
  }




  showResultInfroamtion(employee: EmployeeAddingResponseDto){
    this.employeeInfo.useName = employee.userName;
    this.employeeInfo.password = employee.password;
    this.employeeInfo.email = employee.email;
    this.isDialogShown = true;
  }

  onAppointmentFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
  
    if (input.files && input.files.length > 0) {
      this.appointmentDecisionFile = input.files[0];
    }
  }

  onFaceImageFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    
    if (input.files && input.files.length > 0) {
      this.faceImageFile = input.files[0];
      console.log(input.files[0]);
    }
  }
  

  close() {
    this.visibleChange.emit(false); 
  }
}
