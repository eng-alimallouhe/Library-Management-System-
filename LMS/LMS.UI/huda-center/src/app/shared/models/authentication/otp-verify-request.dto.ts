export interface OtpVerifyRequest{
    email: string;
    code: string;
    codeType: number;
}