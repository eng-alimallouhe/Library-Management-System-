export interface IncentiveOverviewDto {
    incentiveId: string;
    employeeId: string;
    employeeName: string;
    amount: number;
    reason: string;
    decisionDate: string; 
    isPaid: boolean;
    isApproved: boolean;
}