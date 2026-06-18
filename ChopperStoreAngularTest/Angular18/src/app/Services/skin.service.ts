import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Response, Skin, SkinCreateUpdate } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SkinService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getAll(storefrontOnly = false): Observable<Response<Skin[]>> {
    return this.http.get<Response<Skin[]>>(`${this._baseUrl}api/Skins?storefrontOnly=${storefrontOnly}`);
  }

  getById(id: number): Observable<Response<Skin>> {
    return this.http.get<Response<Skin>>(`${this._baseUrl}api/Skins/${id}`);
  }

  getByCategory(categoryId: number, storefrontOnly = false): Observable<Response<Skin[]>> {
    return this.http.get<Response<Skin[]>>(
      `${this._baseUrl}api/Skins/by-category/${categoryId}?storefrontOnly=${storefrontOnly}`
    );
  }

  create(dto: SkinCreateUpdate): Observable<Response<Skin>> {
    return this.http.post<Response<Skin>>(`${this._baseUrl}api/Skins`, dto);
  }

  update(id: number, dto: SkinCreateUpdate): Observable<Response<Skin>> {
    return this.http.put<Response<Skin>>(`${this._baseUrl}api/Skins/${id}`, dto);
  }

  delete(id: number): Observable<Response<Skin>> {
    return this.http.delete<Response<Skin>>(`${this._baseUrl}api/Skins/${id}`);
  }
}
