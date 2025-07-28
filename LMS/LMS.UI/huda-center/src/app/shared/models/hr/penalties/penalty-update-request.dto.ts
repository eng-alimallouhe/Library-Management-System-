export interface PenaltyUpdateRequestDto {
    amount: number;
    reason: string;
    decision?: File;
}