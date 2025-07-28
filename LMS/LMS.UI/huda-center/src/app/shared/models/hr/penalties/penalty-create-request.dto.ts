export interface PenaltyCreateRequestDto {
    employeeId: string;
    amount: number;
    reason: string;
    decisionFile: File; 
}