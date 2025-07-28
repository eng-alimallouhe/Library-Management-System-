import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { DOCUMENT } from '@angular/common';
import { Inject } from '@angular/core';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, CommonModule],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'huda-center';


  constructor(@Inject(DOCUMENT) private document: Document) {
    document.addEventListener('DOMContentLoaded', () => {
        window.addEventListener('load', () => {
          document.querySelectorAll('.material-icons').forEach(icon => {
            (icon as HTMLElement).style.visibility = 'visible';
          });
        });
      });
  }
}
