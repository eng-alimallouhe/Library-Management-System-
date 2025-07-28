import { Component, EventEmitter, Input, Output, OnInit } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { finalize } from 'rxjs';

import { DepartmentService } from '../../../../services/hr/department.service';
import { DepartmentRequestDto } from '../../../../shared/models/hr/departments/department-request.dto';

import { LoaderComponent } from "../../../../shared/components/loader/loader.component";
import { AlterComponent } from "../../../../shared/components/alter/alter.component";

@Component({
  selector: 'app-add-department',
  standalone: true,
  imports: [
    TranslatePipe,
    CommonModule,
    ReactiveFormsModule,
    LoaderComponent,
    AlterComponent
  ],
  templateUrl: './add-department.component.html',
  styleUrl: './add-department.component.css'
})
export class AddDepartmentComponent implements OnInit {
  addForm!: FormGroup;
  isLoadingResult: boolean = false;
  isSubmited: boolean = false; 

  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false; 

  @Output() visibleChange = new EventEmitter<boolean>();
  @Input() visible = false;
  @Output() departmentAdded = new EventEmitter<void>(); 

  constructor(
    private departmentService: DepartmentService,
    private translate: TranslateService,
    private fb: FormBuilder
  ) { }

  ngOnInit() {
    this.addForm = this.fb.group({
      departmentName: ['', [
        Validators.required,
        Validators.maxLength(100)
      ]],
      departmentDescription: ['', [
        Validators.maxLength(500)
      ]]
    });
  }

  onSubmit() {
    this.isSubmited = true;

    if (this.addForm.invalid) {
      console.log('Form is invalid. Not submitting.');
      return;
    }

    const request: DepartmentRequestDto = {
      departmentName: this.addForm.value.departmentName,
      departmentDescription: this.addForm.value.departmentDescription || ''
    };

    this.isLoadingResult = true;
    console.log('Submitting department creation request...');

    this.departmentService.addDepartment(request)
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
          
          this.translate.get(response.statusKey)
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true; 
              console.log('Alert message set. alertVisible set to true.');
            });
          if (response.isSuccess) {
            this.addForm.reset(); 
            this.isSubmited = false; 
            this.departmentAdded.emit(); 
            console.log('Department added successfully. departmentAdded event emitted.');
          }
        },
        error: error => {
          this.isSuccessAlert = false; 
          console.error('API Error:', error);
          this.translate.get(error.error.statusKey || 'UNKNOWN_ERROR')
            .subscribe(translatedMessage => {
              this.alertMessage = translatedMessage;
              this.alertVisible = true; 
              console.log('Error alert message set. alertVisible set to true.');
            });
        }
      });
  }

  onAlertClosed(): void {
    if (this.isSuccessAlert) {
      this.close(); 
    } else {
    }
  }

  close() {
    this.addForm.reset(); 
    this.isSubmited = false; 
    this.visibleChange.emit(false);
  }

  getControl(name: string) {
    return this.addForm.get(name);
  }

  showError(controlName: string): boolean {
    const control = this.getControl(controlName);
    return (control?.invalid && (control?.dirty || control?.touched || this.isSubmited)) ?? false;
  }
}
