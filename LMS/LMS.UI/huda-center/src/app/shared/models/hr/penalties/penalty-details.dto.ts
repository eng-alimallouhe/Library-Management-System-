export interface PenaltyDetailsDto {
    penaltyId: string;
    employeeId: string;
    employeeName: string;
    amount: number;
    reason: string;
    decisionDate: string;
    decisionFileUrl: string;
}