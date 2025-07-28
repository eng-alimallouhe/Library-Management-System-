import { Injectable } from '@angular/core';
import { environment } from '../../../environments/environment';
import { HttpClient } from '@angular/common/http';
import { PagedResult } from '../../shared/models/results/paged-result.dto';
import { NotificationOverviewDto } from '../../shared/models/identity/notification-overview.dto';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class NotificationsService {
  apiUrl = environment.apiUrl + 'identity/Notification';

  constructor(private http: HttpClient) { }

  
  getUnreadNotifications(userId: string, pageNumber: number, pageSize: number) : Observable<PagedResult<NotificationOverviewDto>> {
    return this.http.get<PagedResult<NotificationOverviewDto>>(`${this.apiUrl}/unread?userId=${userId}&pageNumber=${pageNumber}&pageSize=${pageSize}`);
  }

  getUnreadNotificationsCount(userId: string) : Observable<number> {
    return this.http.get<number>(`${this.apiUrl}/unread/count?userId=${userId}`);
  }

  readNotification(notificationId: string){
    return this.http.put(`${this.apiUrl}/${notificationId}`, notificationId);
  }
}