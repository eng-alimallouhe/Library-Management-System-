export interface StockSnapshotDto {
  productId: string;
  productName: string;
  productStock: number;
  productPrice: number;
  totalValue: number; 
  updatedAt: string; 
  logsCount: number;
}