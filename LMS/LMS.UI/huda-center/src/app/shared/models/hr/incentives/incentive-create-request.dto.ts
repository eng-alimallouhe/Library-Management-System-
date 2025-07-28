export interface IncentiveCreateRequestDto {
    employeeId: string;
    amount: number;
    reason: string;
    decisionFile: File; 
}