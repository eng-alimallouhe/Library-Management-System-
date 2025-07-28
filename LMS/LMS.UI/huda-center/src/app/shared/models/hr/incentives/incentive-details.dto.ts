export interface IncentiveDetailsDto {
    incentiveId: string;
    employeeId: string;
    employeeName: string;
    amount: number;
    reason: string;
    decisionDate: string;
    decisionFileUrl: string;
    isApproved: boolean;
    isPaid: boolean;
}