import { Component } from '@angular/core';
import { MenueComponent } from "../../shared/components/menue/menue.component";
import { NavBarComponent } from "../../shared/components/nav-bar/nav-bar.component";
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-main-layout',
  imports: [MenueComponent, NavBarComponent, RouterOutlet],
  templateUrl: './main-layout.component.html',
  styleUrl: './main-layout.component.css'
})
export class MainLayoutComponent {

}
