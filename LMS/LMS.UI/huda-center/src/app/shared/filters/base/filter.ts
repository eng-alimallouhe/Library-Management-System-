export interface Filter {
  name?: string | null;
  language: number;
  from?: Date | string | null;
  to?: Date | string | null;   
  onlyActiveRegisters?: boolean;
  pageNumber?: number;
  pageSize?: number;
}