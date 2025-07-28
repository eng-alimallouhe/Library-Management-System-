// src/app/shared/components/alter/alter.component.ts

import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core'; // لا نحتاج OnChanges, SimpleChanges
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-alert',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe
  ],
  templateUrl: './alter.component.html', // هذا هو قالب الـ modal alert
  styleUrl: './alter.component.css' // هذا هو CSS الـ modal alert
})
export class AlterComponent {
  @Input() visible = false;
  @Input() message = '';
  @Input() isSuccess = false; // جديد: لتحديد نوع التنبيه (نجاح/فشل)

  @Output() visibleChange = new EventEmitter<boolean>();
  @Output() alertClosed = new EventEmitter<void>(); // لإطلاق حدث عند إغلاق التنبيه

  close() {
    this.visible = false;
    this.visibleChange.emit(false); // إبلاغ الربط ثنائي الاتجاه أن التنبيه أصبح غير مرئي
    this.alertClosed.emit(); // إطلاق حدث إغلاق التنبيه
  }
}