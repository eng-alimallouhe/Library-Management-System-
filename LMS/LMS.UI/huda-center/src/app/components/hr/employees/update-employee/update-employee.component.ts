import { Component, EventEmitter, Input, Output, ɵTracingSnapshot } from '@angular/core';
import { EmployeeUpdateRequestDto } from '../../../../shared/models/hr/employee/employee-update-request.dto';
import { EmployeeSnapshotDto } from '../../../../shared/models/hr/employee/employee-snapshot.dto';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs';
import { EmployeeService } from '../../../../services/hr/employee.service';
import { AlterComponent } from "../../../../shared/components/alter/alter.component";
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-update-employee',
  imports: [
    TranslatePipe,
    CommonModule,
    ReactiveFormsModule,
    AlterComponent,
    LoaderComponent
],
  templateUrl: './update-employee.component.html',
  styleUrl: './update-employee.component.css'
})
export class UpdateEmployeeComponent {
  updateForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmited: boolean = false;
  faceImageFile: File | undefined;
  employeeInformation!: EmployeeSnapshotDto;

  isLoadingDepartments = true;
  
  alertVisible = false;
  alertMessage = '';

  isResultLoaded = false;

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Input() employeeId: string = '';


  constructor(
    private employeeService: EmployeeService,
    private translate: TranslateService,
    private fb: FormBuilder,
    private router: Router
  ) { }


  ngOnInit() {
    this.updateForm = this.fb.group({
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
      
      baseSalary: ['', [
        Validators.required
      ]]
    });

    this.employeeService.getEmployeeSnapshot(this.employeeId)
    .subscribe({
      next: response => {
        this.employeeInformation = response;
        this.updateForm.get('fullName')?.setValue(response.fullName);
        this.updateForm.get('email')?.setValue(response.email);
        this.updateForm.get('phoneNumber')?.setValue(response.phoneNumber);
        this.updateForm.get('baseSalary')?.setValue(response.baseSalary);
      },
      error: error => {

      }
    });
  }


  onSubmit(){
    const request : EmployeeUpdateRequestDto = {
      fullName: this.updateForm.value.fullName,
      email: this.updateForm.value.email,
      phoneNumber: this.updateForm.value.phoneNumber,
      baseSalary: this.updateForm.value.baseSalary
    };
    
    if (this.faceImageFile) {
      request.employeeFaceImage = this.faceImageFile;
    }

    if (this.updateForm.invalid) {
      this.isSubmited = true;
      console.log('false');
      
      return;
    }
    this.isSubmited = false;

    this.isLoadingResult = true;

    this.employeeService.updateEmployee(this.employeeId, request)
    .pipe(
      finalize( () => {this.isLoadingResult = false 
      }))
    .subscribe({
      next: response => {
        this.translate.get(response.statusKey)
        .subscribe(translatedMessage => {
          this.alertMessage = translatedMessage;
          this.alertVisible = true;
        });
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
