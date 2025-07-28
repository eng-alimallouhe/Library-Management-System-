export interface ProductToUpdateDto {
  productId: string;
  arProductName: string;
  arProductDescription: string;
  enProductName: string;
  enProductDescription: string;
  productPrice: number;
  productStock: number;
  categoriesIds: string[]; 
  imgUrl: string;
}
