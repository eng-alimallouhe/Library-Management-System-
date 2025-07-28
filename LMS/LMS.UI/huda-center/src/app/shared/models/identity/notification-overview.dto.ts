export interface NotificationOverviewDto{
    notificationId: string;
    title: string;
    message: string;
    createdAt: string;
    isRead: boolean;
    redirectUrl: string | null;
}