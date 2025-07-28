import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from '../../../environments/environment';


@Injectable({
  providedIn: 'root'
})
export class NotificationHubService {
  private hubConnection!: signalR.HubConnection;

  constructor() {
    this.startConnection();
  }

  private startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubUrl}notificationHub`)
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch(err => console.error('SignalR Connection Error: ', err));
  }

  
  public onNotificationReceived(callback: (notification: any) => void): void {
    this.hubConnection.on('ReceiveNotification', callback);
  }

  public offNotificationReceived(callback: (notification: any) => void): void {
    this.hubConnection.off('ReceiveNotification', callback);
  }
}
