import { Filter } from "../base/filter";

export interface ProductFilter extends Filter {
  maxPrice?: number;
  minPrice?: number;
  categoryId?: string[];
  maxQuantity?: number;
  minQuantity?: number;
}