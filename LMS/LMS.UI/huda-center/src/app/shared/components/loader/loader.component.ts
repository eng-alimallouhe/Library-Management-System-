import { CommonModule } from '@angular/common';
import { Component, Input, OnInit } from '@angular/core';
import { TranslatePipe } from '@ngx-translate/core';

@Component({
  selector: 'app-loader',
  imports: [
    TranslatePipe,
    CommonModule
  ],
  templateUrl: './loader.component.html',
  styleUrl: './loader.component.css'
})
export class LoaderComponent{
  @Input() type : 'circle' | 'bar' = 'bar';
  @Input() messageKey: string = 'COMMON.LOADING';
  @Input() height: string = '300px';
  @Input() width: string = '300px';
}