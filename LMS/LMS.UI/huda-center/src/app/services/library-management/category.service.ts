import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CategoryLookUpDto } from '../../shared/models/library-management/categories/category-lookup.dto';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class CategoryService {
  private baseUrl = environment.apiUrl + 'stock/Category';

  constructor(private http: HttpClient) { }


  getCategoryLookup(language: number): Observable<CategoryLookUpDto[]> {
    let params = new HttpParams();
    params = params.append('language', language.toString());
    return this.http.get<CategoryLookUpDto[]>(`${this.baseUrl}`, { params });
  }
}