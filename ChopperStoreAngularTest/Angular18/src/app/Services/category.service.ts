import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CategoryCreateUpdate, Response } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class CategoryService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getAll(): Observable<Response<Category[]>> {
    return this.http.get<Response<Category[]>>(`${this._baseUrl}api/Categories`);
  }

  getById(id: number): Observable<Response<Category>> {
    return this.http.get<Response<Category>>(`${this._baseUrl}api/Categories/${id}`);
  }

  create(dto: CategoryCreateUpdate): Observable<Response<Category>> {
    return this.http.post<Response<Category>>(`${this._baseUrl}api/Categories`, dto);
  }

  update(id: number, dto: CategoryCreateUpdate): Observable<Response<Category>> {
    return this.http.put<Response<Category>>(`${this._baseUrl}api/Categories/${id}`, dto);
  }

  delete(id: number): Observable<Response<Category>> {
    return this.http.delete<Response<Category>>(`${this._baseUrl}api/Categories/${id}`);
  }
}
