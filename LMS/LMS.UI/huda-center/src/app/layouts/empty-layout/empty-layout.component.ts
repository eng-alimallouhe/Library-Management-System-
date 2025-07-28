import { Component, OnDestroy, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-empty-layout',
  imports: [RouterOutlet],
  templateUrl: './empty-layout.component.html',
  styleUrl: './empty-layout.component.css'
})
export class EmptyLayoutComponent implements OnInit, OnDestroy {
  ngOnInit(): void {
    console.log('init empty');
    
  }
  ngOnDestroy(): void {
    console.log('destory empty');
  }

}
