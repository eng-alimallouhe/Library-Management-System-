import { Injectable } from '@angular/core';
import { ProductFilter } from '../filters/library-management/products.filter';
import { EmployeeFilter } from '../filters/hr/employee.filter';
import { DepartmentFilter } from '../filters/hr/department.filter';
import { PenaltyFilter, PenaltyOrderBy } from '../filters/hr/penalty-filter';
import { Params } from '@angular/router';
import { IncentiveFilter, IncentiveOrderBy } from '../filters/hr/incentive.filter';
import { LeaveFilter } from '../filters/hr/leave.filter';
import { LeaveType } from '../models/hr/leaves/leave-type.enum';


@Injectable({
  providedIn: 'root'
})
export class QuerySupporterService {
  constructor() { }

  getProductsQueryParameters(filter: ProductFilter) : any {
    const query: any = {};

    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        query[key] = value;
      }
    });
    return query;
  }

  getDepartmentsQueryParams(filter: DepartmentFilter) : any {
    const query: any = {};
    
    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        query[key] = value;
      }
    });
    return query;
  }
  

  getEmployeesQueryParameters(filter: EmployeeFilter) : any {
    const query: any = {};

    Object.entries(filter).forEach(([key, value]) => {
      if (value !== null && value !== undefined && value !== '') {
        query[key] = value;
      }
    });
    return query;
  }

  private getPenaltyOrderByString(orderBy: PenaltyOrderBy): string {
        switch (orderBy) {
            case PenaltyOrderBy.ByEmployeeName:
                return 'ByEmployeeName';
            case PenaltyOrderBy.ByAmount:
                return 'ByAmount';
            case PenaltyOrderBy.ByReason:
                return 'ByReason';
            case PenaltyOrderBy.ByDate:
                return 'ByDate';
            case PenaltyOrderBy.ByIsApplied:
                return 'ByIsApplied';
            case PenaltyOrderBy.ByIsApproved:
                return 'ByIsApproved';
            default:
                return 'ByDate'; // قيمة افتراضية في حالة عدم التطابق
        }
    }

  getPenaltiesQueryParams(filter: PenaltyFilter): Params {
      const params: Params = {};

      if (filter.pageNumber !== undefined && filter.pageNumber !== null) {
          params['pageNumber'] = filter.pageNumber;
      }
      if (filter.pageSize !== undefined && filter.pageSize !== null) {
          params['pageSize'] = filter.pageSize;
      }
      if (filter.departmentIds && filter.departmentIds.length > 0) {
          params['departmentIds'] = filter.departmentIds;
      }
      if (filter.byIsApplied !== undefined && filter.byIsApplied !== null) {
          params['byIsApplied'] = filter.byIsApplied;
      }
      if (filter.byIsApproved !== undefined && filter.byIsApproved !== null) {
          params['byIsApproved'] = filter.byIsApproved;
      }
      if (filter.isDesc !== undefined && filter.isDesc !== null) {
          params['isDesc'] = filter.isDesc;
      }
      if (filter.orderBy !== undefined && filter.orderBy !== null) {
          params['orderBy'] = this.getPenaltyOrderByString(filter.orderBy);
      }
      if (filter.name) {
        params['name'] = filter.name;
      }
      return params;
  }

  private getIncentiveOrderByString(orderBy: IncentiveOrderBy): string {
        switch (orderBy) {
            case IncentiveOrderBy.ByEmployeeName:
                return 'ByEmployeeName';
            case IncentiveOrderBy.ByReason:
                return 'ByReason';
            case IncentiveOrderBy.ByDate:
                return 'ByDate';
            case IncentiveOrderBy.ByIsPaid:
                return 'ByIsApplied';
            case IncentiveOrderBy.ByIsApproved:
                return 'ByIsApproved';
            default:
                return 'ByDate'; // قيمة افتراضية في حالة عدم التطابق
        }
    }

  getIncentivesQueryParams(filter: IncentiveFilter): Params {
      const params: Params = {};

      if (filter.pageNumber !== undefined && filter.pageNumber !== null) {
          params['pageNumber'] = filter.pageNumber;
      }
      if (filter.pageSize !== undefined && filter.pageSize !== null) {
          params['pageSize'] = filter.pageSize;
      }
      if (filter.departmentIds && filter.departmentIds.length > 0) {
          params['departmentIds'] = filter.departmentIds;
      }
      if (filter.byIsPaid !== undefined && filter.byIsPaid !== null) {
          params['byIsApplied'] = filter.byIsPaid;
      }
      if (filter.byIsApproved !== undefined && filter.byIsApproved !== null) {
          params['byIsApproved'] = filter.byIsApproved;
      }
      if (filter.isDesc !== undefined && filter.isDesc !== null) {
          params['isDesc'] = filter.isDesc;
      }
      if (filter.orderBy !== undefined && filter.orderBy !== null) {
          params['orderBy'] = this.getIncentiveOrderByString(filter.orderBy);
      }
      if (filter.name) {
        params['name'] = filter.name;
      }
      return params;
  }

  getLeavesQueryParams(filter: LeaveFilter): Params {
    const queryParams: Params = {};

    if (filter.pageNumber) queryParams['pageNumber'] = filter.pageNumber;
    if (filter.pageSize) queryParams['pageSize'] = filter.pageSize;
    if (filter.name) queryParams['name'] = filter.name;
    if (filter.from) queryParams['from'] = filter.from;
    if (filter.to) queryParams['to'] = filter.to;
    if (filter.status !== undefined && filter.status !== null) queryParams['status'] = filter.status;

    // هنا يتم التحويل من القيمة الرقمية للـ enum إلى اسمها النصي
    if (filter.type !== undefined && filter.type !== null) {
      queryParams['type'] = LeaveType[filter.type]; // تحويل الرقم إلى اسم النص
    }

    if (filter.isPaid !== undefined && filter.isPaid !== null) queryParams['isPaid'] = filter.isPaid;
    if (filter.orderBy) queryParams['orderBy'] = filter.orderBy;
    if (filter.isDesc !== undefined && filter.isDesc !== null) queryParams['isDesc'] = filter.isDesc;

    return queryParams;
  }
}