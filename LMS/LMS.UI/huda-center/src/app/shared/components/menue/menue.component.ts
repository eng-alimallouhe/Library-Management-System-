import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { TranslatePipe } from '@ngx-translate/core';

interface SubMenuItem {
  text: string;
  route: string;
  requiredRoles?: string[];
}

interface MenuItem {
  icon: string;
  text: string;
  route?: string;
  subItems?: SubMenuItem[];
  isExpanded?: boolean;
  requiredRoles?: string[];
}

@Component({
  selector: 'app-menue',
  imports: [RouterLink,
    RouterLinkActive,
    CommonModule,
    TranslatePipe
  ],
  templateUrl: './menue.component.html',
  styleUrls: ['./menue.component.css']
})
export class MenueComponent implements OnInit {
  currentUserRole: string | null = null;

  menuItems: MenuItem[] = [
    { icon: 'dashboard', text: 'لوحة التحكم', route: '/dashboard', requiredRoles: ['admin', 'employee', 'customer'] },
    {
      icon: 'business',
      text: 'الأقسام',
      subItems: [
        { text: 'جميع الأقسام', route: '/admin/departments/all' },
        { text: 'إضافة قسم', route: '/admin/departments/add' }
      ],
      requiredRoles: ['admin', 'customer'],
      isExpanded: false
    },
    {
      icon: 'store',
      text: 'إدارة المخزون',
      subItems: [
        { text: 'نظرة عامة على المخزون', route: '/stock/snapshot' },
        { text: 'تحليل المخزون الراكد', route: '/stock/dead' }
      ],
      isExpanded: false
    },
    { icon: 'person', text: 'ملفي الشخصي', route: '/profile' },
    { icon: 'settings', text: 'الإعدادات', route: '/settings' },
    { icon: 'logout', text: 'تسجيل الخروج', route: '/logout' }
  ];

  // قم بتعيين isMenuOpen إلى false بشكل افتراضي لكي تكون القائمة مغلقة
  isMenuOpen: boolean = false;
  isMobile: boolean = false;
  isMobileFirstTime = false;

  constructor() { }

  ngOnInit(): void {
    // لا نحتاج لاستدعاء checkScreenSize هنا إذا أردنا القائمة مغلقة دائمًا بشكل افتراضي
    // ولكن سنبقيها لتحديث isMobile بشكل صحيح إذا كان هناك أي منطق آخر يعتمد عليها
    this.checkScreenSize();
    this.currentUserRole = 'admin'; // قم بتحديث هذا بناءً عل
  }

  @HostListener('window:resize', ['$event'])
  onResize(event: Event): void {
    this.checkScreenSize();
  }

  private checkScreenSize(): void {
    this.isMobile = window.innerWidth <= 991;
    this.menuItems.forEach(item => {
      if (item.subItems) {
        item.isExpanded = false;
      }
    });

    if (this.isMobile) {
      this.isMobileFirstTime = true;
      setTimeout(() => {
        this.isMobileFirstTime = false;
      }, 2000);
    }
  }

  toggleMenu(): void {
    this.isMenuOpen = !this.isMenuOpen;
  }

  closeMenu(): void {
    this.isMenuOpen = false;
  }

  toggleSubMenu(item: MenuItem): void {
    if (item.subItems) {
      this.menuItems.forEach(menuItem => {
        if (menuItem !== item && menuItem.isExpanded) {
          menuItem.isExpanded = false;
        }
      });
      item.isExpanded = !item.isExpanded;
    }
  }

  closeMenuOnMobileLinkClick(): void {
    setTimeout(() => {
      this.isMenuOpen = false;
    }, 100);
  }

  hasRequiredRole(item: MenuItem | SubMenuItem): boolean {
    if (!item.requiredRoles || item.requiredRoles.length === 0) {
      return true;
    }

    if (!this.currentUserRole) {
      return false;
    }

    return item.requiredRoles.includes(this.currentUserRole);
  }
}