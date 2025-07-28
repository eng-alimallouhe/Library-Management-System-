import { OrderOverviewDto } from "../../orders/order-overview.dto";
import { EmployeeOverViewDto } from "../employee/employee-overview.dto";

export interface DepartmentDetailsDto{
     departmentName :  string;
     departmentDescription :  string;
     currentEmployees: EmployeeOverViewDto[];
     formerEmployees: EmployeeOverViewDto[];
     currentOrders: OrderOverviewDto[];
}