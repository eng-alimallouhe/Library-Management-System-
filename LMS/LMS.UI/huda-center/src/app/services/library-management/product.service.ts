// src/app/services/library-management/product.service.ts

import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ProductCreateRequestDto } from '../../shared/models/library-management/products/product-create-request.dto';
import { ProductDetailsDto } from '../../shared/models/library-management/products/product-details.dto';
import { ProductOverviewDto } from '../../shared/models/library-management/products/product-overview.dto';
import { ProductToUpdateDto } from '../../shared/models/library-management/products/product-to-update.dto';
import { ProductUpdateRequestDto } from '../../shared/models/library-management/products/product-update-request.dto';
import { Result } from '../../shared/models/result/result';
import { PagedResult } from '../../shared/models/results/paged-result.dto';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class ProductService {
  private baseUrl = environment.apiUrl + 'stock/product' // تأكد من أن هذا يتطابق مع مسار الـ API الخاص بك

  constructor(private http: HttpClient) { }

  // جلب جميع المنتجات مع ترقيم (بدون فلاتر حالياً)
  // سنستخدم ProductFilter ولكن فقط لـ pageNumber و pageSize
  getAllProducts(pageNumber: number, pageSize: number): Observable<PagedResult<ProductOverviewDto>> {
    let params = new HttpParams();
    params = params.append('PageNumber', pageNumber.toString());
    params = params.append('PageSize', pageSize.toString());
    // لا نرسل أي فلاتر أخرى حالياً
    return this.http.get<PagedResult<ProductOverviewDto>>(`${this.baseUrl}`, { params });
  }

  // جلب تفاصيل منتج واحد
  getProductById(id: string, language: number): Observable<ProductDetailsDto> {
    let params = new HttpParams();
    params = params.append('language', language.toString());
    return this.http.get<ProductDetailsDto>(`${this.baseUrl}/${id}`, { params });
  }

  // جلب بيانات منتج للتعديل
  getProductToUpdate(id: string, language: number): Observable<ProductToUpdateDto> {
    let params = new HttpParams();
    params = params.append('language', language.toString());
    return this.http.get<ProductToUpdateDto>(`${this.baseUrl}/to-update/${id}`, { params });
  }

  // إضافة منتج جديد
  addProduct(product: ProductCreateRequestDto): Observable<Result> {
    const formData = new FormData();
    formData.append('ARProductName', product.aRProductName);
    formData.append('ARProductDescription', product.aRProductDescription);
    formData.append('ENProductName', product.eNProductName);
    formData.append('ENProductDescription', product.eNProductDescription);
    formData.append('ProductPrice', product.productPrice.toString());
    formData.append('ProductStock', product.productStock.toString());
    product.categoriesIds.forEach(id => formData.append('CategoriesIds', id));
    if (product.imageFile) {
      formData.append('ImageFile', product.imageFile, product.imageFile.name);
    }
    return this.http.post<Result>(`${this.baseUrl}`, formData);
  }

  updateProduct(id: string, product: ProductUpdateRequestDto): Observable<Result> {
    const formData = new FormData();
    formData.append('ARProductName', product.arProductName);
    formData.append('ARProductDescription', product.arProductDescription);
    formData.append('ENProductName', product.enProductName);
    formData.append('ENProductDescription', product.enProductDescription);
    formData.append('ProductPrice', product.productPrice.toString());
    formData.append('ProductStock', product.productStock.toString());
    product.categoriesIds.forEach(id => formData.append('CategoriesIds', id));
    if (product.imageFile) {
      formData.append('ImageFile', product.imageFile, product.imageFile.name);
    }
    return this.http.put<Result>(`${this.baseUrl}/${id}`, formData);
  }

  deleteProduct(id: string): Observable<Result> {
    return this.http.delete<Result>(`${this.baseUrl}/${id}`);
  }
}
