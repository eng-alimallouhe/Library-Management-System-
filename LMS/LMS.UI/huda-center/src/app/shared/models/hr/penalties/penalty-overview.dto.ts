export interface PenaltyOverviewDto {
    penaltyId: string;
    employeeId: string;
    employeeName: string;
    amount: number;
    reason: string;
    decisionDate: string; 
    isApplied: boolean;
    isApproved: boolean;
}