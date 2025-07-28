// src/app/shared/models/library-management/products/product-update-request.dto.ts

export interface ProductUpdateRequestDto {
  arProductName: string;
  arProductDescription: string;
  enProductName: string;
  enProductDescription: string;
  productPrice: number;
  productStock: number;
  categoriesIds: string[];
  imageFile: File | null; // لرفع الملف (يمكن أن يكون null إذا لم يتم تغيير الصورة)
}
