import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-employee-credential-dialog',
  imports: [
    TranslatePipe,
    CommonModule
  ],
  templateUrl: './employee-credential-dialog.component.html',
  styleUrl: './employee-credential-dialog.component.css'
})
export class EmployeeCredentialDialogComponent {
  @Input() userName!: string;
  @Input() email!: string;
  @Input() password!: string;

  @Input() isVisible = false;

  @Output() isVisibleChange = new EventEmitter<boolean>();

  onClosedClicked() {
    this.isVisible = false;
    this.isVisibleChange.emit();
  }
}
