// src/app/shared/models/library-management/products/product-overview.dto.ts

export interface ProductOverviewDto {
  productId: string;
  productName: string; // هذا سيكون الاسم باللغة المختارة (عربي أو إنجليزي)
  productPrice: number;
  productStock: number;
  imgUrl: string;
  productDescription: string;
  discountPercentage: number; // <--- الخاصية الجديدة
}
