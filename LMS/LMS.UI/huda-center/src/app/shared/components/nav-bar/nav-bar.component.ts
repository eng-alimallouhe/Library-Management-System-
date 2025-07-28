import { CommonModule } from '@angular/common';
import { Component, OnInit, HostListener, Input, OnDestroy } from '@angular/core';
import { TranslatePipe, TranslateService } from '@ngx-translate/core';
import { NotificationOverviewDto } from '../../models/identity/notification-overview.dto';
import { NotificationsService } from '../../../services/identity/notifications.service';
import { NotificationHubService } from '../../services/notification-hub.service';
import { Router } from '@angular/router';
import { debounceTime, finalize, Subject } from 'rxjs';
import { LoaderComponent } from "../loader/loader.component";
import { ThemeService } from '../../services/theme.service';
import { SearchCommunicationService } from '../../services/search-communication.service';

@Component({
  selector: 'app-nav-bar',
  standalone: true,
  imports: [
    CommonModule,
    TranslatePipe,
    LoaderComponent
  ],
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css']
})
export class NavBarComponent implements OnInit, OnDestroy {
  currentMode: 'light_mode' | 'dark_mode' = 'light_mode';
  showModesContainer = false;
  showSearchInput = false;
  isLoadingNotification = false;

  userId = 'F3F4A526-3DCC-4A12-9557-F3C8BED49117';
  notifications: NotificationOverviewDto[] = [];
  unreadNotificationsCount: number = 0;
  showNotificationsDropdown = false;
  pageSize = 10;
  hasMore = false;
  currentPage = 1;

  @Input() isSideMenuExpanded: boolean = true;

  showLanguageDropdown = false;
  currentLanguage: string = 'ar';
  availableLanguages = ['English', 'العربية'];

  showUserDropdown = false;
  userProfileImageUrl: string = 'https://static.vecteezy.com/system/resources/previews/036/280/651/original/default-avatar-profile-icon-social-media-user-image-gray-avatar-icon-blank-profile-silhouette-illustration-vector.jpg';

  cartItemCount: number = 0; 
  
  private inputSubject = new Subject<string>();

  constructor(
    private notificationService: NotificationsService,
    private translateService: TranslateService,
    private notificationHub: NotificationHubService,
    private router: Router,
    private themeService: ThemeService,
    private searchService: SearchCommunicationService
  ) {
    this.inputSubject.pipe(debounceTime(300)).subscribe(value => {
      this.searchService.updateNameSearch(value.trim());
    });
  }

  private handleNotification = (notification: NotificationOverviewDto) => {
    this.unreadNotificationsCount++;

    if (this.showNotificationsDropdown) {
      this.notifications.unshift(notification);
    }
  };

  onInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    const value = input.value;
    this.inputSubject.next(value);
  }

  ngOnInit() {
    this.notificationService.getUnreadNotificationsCount(this.userId)
      .subscribe({
        next: response => {
          this.unreadNotificationsCount = response;
        },
        error: error => {
          console.error("Error fetching unread notifications count:", error);
        }
      });

    this.themeService.applySavedTheme();
    this.notificationHub.onNotificationReceived(this.handleNotification);
  }

  ngOnDestroy(): void {
    this.notificationHub.offNotificationReceived(this.handleNotification);
    this.inputSubject.unsubscribe();
  }

  loadNotifications() {
    this.isLoadingNotification = true;
    this.notificationService.getUnreadNotifications(this.userId, this.currentPage, this.pageSize)
      .pipe(finalize(() => {
        this.isLoadingNotification = false;
      }))
      .subscribe(response => {
        this.notifications = [...this.notifications, ...response.items];
        this.hasMore = response.hasNext;
        this.currentPage++;
      }, error => {
        console.error("Error loading notifications:", error);
      });
  }

  loadMoreNotifications(event: Event) {
    event.stopPropagation();
    this.loadNotifications();
  }

  
  toggleModesContainer() {
    this.showModesContainer = !this.showModesContainer;
    this.closeAllOtherDropdowns('modes');
  }

  applyTheme(theme: 'light_mode' | 'dark_mode') {
    this.currentMode = theme;
    this.themeService.applyTheme(theme);
    this.showModesContainer = false;
  }

  toggleSearchInput() {
    this.showSearchInput = !this.showSearchInput;
    this.closeAllOtherDropdowns('search');
  }

  hideSearchInput() {
    this.showSearchInput = false;
  }

  toggleNotificationsDropdown() {
    this.showNotificationsDropdown = !this.showNotificationsDropdown;

    if (this.showNotificationsDropdown && this.notifications.length === 0) {
      this.loadNotifications();
    }
    this.closeAllOtherDropdowns('notifications');
  }

  onNotificationClick(notification: NotificationOverviewDto, event: MouseEvent) {
    event.stopPropagation();

    this.notificationService.readNotification(notification.notificationId).subscribe({
      next: () => {
        this.notificationService.getUnreadNotificationsCount(this.userId)
          .subscribe({
            next: response => {
              this.unreadNotificationsCount = response;
            },
            error: error => {
              console.error("Error fetching unread notifications count after read:", error);
            }
          });

        if (notification.redirectUrl) {
          this.router.navigate([`${notification.redirectUrl}`]);
        }
      },
      error: (err) => {
        console.error("Error reading notification:", err);
      }
    });
  }

  toggleLanguageDropdown() {
    this.showLanguageDropdown = !this.showLanguageDropdown;
    this.closeAllOtherDropdowns('language');
  }

  changeLanguage(lang: string) {
    this.currentLanguage = lang;
    this.translateService.use(lang);
    localStorage.setItem('currentLang', lang);
    this.showLanguageDropdown = false;
  }

  toggleUserDropdown() {
    this.showUserDropdown = !this.showUserDropdown;
    this.closeAllOtherDropdowns('user');
  }

  navigateToProfile() {
    this.router.navigate(['/profile']);
    this.showUserDropdown = false;
  }

  logout() {
    this.router.navigate(['/login']);
    this.showUserDropdown = false;
  }

  navigateToCart() {
    this.router.navigate(['/cart']); 
    this.closeAllOtherDropdowns('cart');
  }

  closeAllOtherDropdowns(openedDropdown: string) {
    if (openedDropdown !== 'modes') {
      this.showModesContainer = false;
    }
    if (openedDropdown !== 'search') {
      this.showSearchInput = false;
    }
    if (openedDropdown !== 'notifications') {
      this.showNotificationsDropdown = false;
    }
    if (openedDropdown !== 'language') {
      this.showLanguageDropdown = false;
    }
    if (openedDropdown !== 'user') {
      this.showUserDropdown = false;
    }
  }

  @HostListener('document:click', ['$event'])
  onClick(event: Event) {
    const target = event.target as HTMLElement;

    if (this.showModesContainer && !target.closest('.theme-container') && !target.closest('.modes-container')) {
      this.showModesContainer = false;
    }

    if (this.showSearchInput && !target.closest('.search-icon-wrapper') && !target.closest('.floating-search-container')) {
      this.showSearchInput = false;
    }

    if (this.showNotificationsDropdown && !target.closest('.notifications-container') && !target.closest('.notifications-wrapper')) {
      this.showNotificationsDropdown = false;
    }

    if (this.showLanguageDropdown && !target.closest('.language-container') && !target.closest('.languages-container')) {
      this.showLanguageDropdown = false;
    }

    if (this.showUserDropdown && !target.closest('.user-profile-container') && !target.closest('.user-dropdown')) {
      this.showUserDropdown = false;
    }
  }
}