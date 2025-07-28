export interface EmployeeCreateRequestDto{
    fullName: string;
    email: string;
    phoneNumber: string;
    baseSalary: number;
    language: number;
    departmentId: string;
    appointmentDecision: File;
    FaceImage: File;
}