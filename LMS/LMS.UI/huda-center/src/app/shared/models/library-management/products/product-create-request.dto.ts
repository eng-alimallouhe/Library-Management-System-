export interface ProductCreateRequestDto {
  aRProductName: string;
  aRProductDescription: string;
  eNProductName: string;
  eNProductDescription: string;
  productPrice: number;
  productStock: number;
  categoriesIds: string[];
  imageFile: File | null;
}
