import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, Observable, map } from 'rxjs';
import { CommonModule, AsyncPipe } from '@angular/common';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';

import { DepartmentService } from '../../../../services/hr/department.service';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { LocalDatePipe } from '../../../../pipes/local-date.pipe';
import { ConfirmDialogComponent } from '../../../../shared/components/confirm-dialog/confirm-dialog.component'; 
import { DepartmentDetailsDto } from '../../../../shared/models/hr/departments/departmentd-detail.dto';
import { UpdateDepartmentComponent } from "../update-department/update-department.component";
import { AlterComponent } from "../../../../shared/components/alter/alter.component";

@Component({
  selector: 'app-department-details',
  standalone: true,
  imports: [
    CommonModule,
    AsyncPipe,
    TranslatePipe,
    RouterLink,
    LocalDatePipe,
    LoaderComponent,
    ConfirmDialogComponent,
    UpdateDepartmentComponent,
    AlterComponent
],
  templateUrl: './department.component.html',
  styleUrl: './department.component.css'
})
export class DepartmentComponent implements OnInit {
  departmentId: string = '';
  department$!: Observable<DepartmentDetailsDto>;
  isLoadingDepartment: boolean = true;
  showConfirmDeleteDialog: boolean = false; 
  alertVisible = false;
  alertMessage = '';
  isSuccessAlert = false;
  
  isDepartmentUpdateFormVisible: boolean = false; 

  constructor(
    private departmentService: DepartmentService,
    private route: ActivatedRoute,
    private router: Router,
    private translate: TranslateService
  ) { }

  ngOnInit(): void {
    this.isLoadingDepartment = true;

    this.departmentId = this.route.snapshot.paramMap.get('id') || '';

    if (!this.departmentId) {
      console.error('Department ID is missing in the URL.');
      this.isLoadingDepartment = false;
      return;
    }

    this.department$ = this.departmentService.getDepartment(this.departmentId).pipe(
      map(department => ({
        ...department,
        currentEmployees: department.currentEmployees?.slice().sort((a, b) => a.fullName.localeCompare(b.fullName)),
        formerEmployees: department.formerEmployees?.slice().sort((a, b) => a.fullName.localeCompare(b.fullName)),
        currentOrders: department.currentOrders?.slice().sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      })),
      finalize(() => {
        this.isLoadingDepartment = false;
      })
    );
  }

  onEdit(): void {
    this.isDepartmentUpdateFormVisible = true;
  }

  onDelete(): void {
    this.showConfirmDeleteDialog = true;
  }

  deleteDepartment(): void {
    this.departmentService.deleteDepartment(this.departmentId)
      .subscribe({
        next: response => {
          this.showConfirmDeleteDialog = false;
          this.isSuccessAlert = true;
          this.translate.get(response.statusKey)
          .subscribe(translatedMessage => {
            this.alertMessage = translatedMessage;
          });
          this.alertVisible = true;
          setTimeout(() => {
            this.router.navigate(['/app/hr/departments']);
          }, 3000); 
        },
        error: error => {
          this.showConfirmDeleteDialog = false;
          this.isSuccessAlert = false;
          this.translate.get(error?.error.statusKey)
          .subscribe(translatedMessage => {
            this.alertMessage = translatedMessage;
          });
          this.alertVisible = true;
        }
      });
  }

  onCancelDelete(): void {
    this.showConfirmDeleteDialog = false;
  }

  onDepartmentUpdated(): void {
    this.ngOnInit(); 
    this.isDepartmentUpdateFormVisible = false;
  }
}
