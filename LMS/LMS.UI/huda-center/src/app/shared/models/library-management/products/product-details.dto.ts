import { CategoryLookUpDto } from "../categories/category-lookup.dto"; // تأكد من المسار الصحيح

export interface ProductDetailsDto {
  productId: string;
  arProductName: string;
  arProductDescription: string;
  enProductName: string;
  enProductDescription: string;
  productPrice: number;
  productStock: number;
  imgUrl: string;
  discountPercentage: number;
  categories: CategoryLookUpDto[];
}
