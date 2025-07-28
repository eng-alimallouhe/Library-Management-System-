// src/app/products/products.component.ts

import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslatePipe } from '@ngx-translate/core';
import { AddProductComponent } from '../add-product/add-product.component';
import { ProductService } from '../../../../services/library-management/product.service';
import { LoaderComponent } from '../../../../shared/components/loader/loader.component';
import { ProductOverviewDto } from '../../../../shared/models/library-management/products/product-overview.dto';
import { PagedResult } from '../../../../shared/models/results/paged-result.dto';

@Component({
  selector: 'app-products',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    TranslateModule,
    TranslatePipe,
    LoaderComponent,
    AddProductComponent
],
  templateUrl: './products.component.html',
  styleUrls: ['./products.component.css']
})
export class ProductsComponent implements OnInit {
  products: ProductOverviewDto[] = [];
  currentPage: number = 1;
  pageSize: number = 20;
  totalCount: number = 0;
  totalPages: number = 0;
  isLoading: boolean = false;
  errorMessage: string | null = null;

  isAddProductFormVisible = false; // <--- متغير للتحكم بظهور المودال

  constructor(private productService: ProductService) { }

  ngOnInit(): void {
    this.loadProducts();
  }

  loadProducts(): void {
    this.isLoading = true;
    this.errorMessage = null;
    this.productService.getAllProducts(this.currentPage, this.pageSize).subscribe({
      next: (response: PagedResult<ProductOverviewDto>) => {
        this.products = response.items;
        this.totalCount = response.totalCount;
        this.totalPages = response.totalPages;
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading products:', error);
        this.errorMessage = 'COMMON.ERROR_LOADING_DATA';
        this.isLoading = false;
      }
    });
  }

  goToPreviousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.loadProducts();
    }
  }

  goToNextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.loadProducts();
    }
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.currentPage = page;
      this.loadProducts();
    }
  }

  getPageNumbers(): number[] {
    const pageNumbers: number[] = [];
    const maxPagesToShow = 5;
    let startPage = Math.max(1, this.currentPage - Math.floor(maxPagesToShow / 2));
    let endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);

    if (endPage - startPage + 1 < maxPagesToShow) {
      if (startPage === 1) {
        endPage = Math.min(this.totalPages, startPage + maxPagesToShow - 1);
      } else if (endPage === this.totalPages) {
        startPage = Math.max(1, endPage - maxPagesToShow + 1);
      }
    }

    for (let i = startPage; i <= endPage; i++) {
      pageNumbers.push(i);
    }
    return pageNumbers;
  }

  openAddProductModal(): void {
    this.isAddProductFormVisible = true; // <--- إظهار المودال
  }

  onProductAdded(): void {
    console.log('New product added, reloading products.');
    this.loadProducts(); // <--- إعادة تحميل المنتجات بعد الإضافة الناجحة
  }

  viewProductDetails(productId: string){

  }
}
