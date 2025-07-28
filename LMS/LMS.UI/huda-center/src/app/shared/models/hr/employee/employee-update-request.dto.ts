export interface EmployeeUpdateRequestDto{
    fullName: string;
    email: string;
    phoneNumber: string;
    baseSalary: number;
    employeeFaceImage?: File;
}