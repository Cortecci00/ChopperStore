import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Response, ShoppingCart } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShoppingCartService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getMine(): Observable<Response<ShoppingCart>> {
    return this.http.get<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/mine`);
  }

  addItem(dto: { skinId: number; quantity: number }): Observable<Response<ShoppingCart>> {
    return this.http.post<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/items`, dto);
  }

  updateItem(itemId: number, dto: { quantity: number }): Observable<Response<ShoppingCart>> {
    return this.http.put<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/items/${itemId}`, dto);
  }

  removeItem(itemId: number): Observable<Response<ShoppingCart>> {
    return this.http.delete<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/items/${itemId}`);
  }

  clear(): Observable<Response<string>> {
    return this.http.delete<Response<string>>(`${this._baseUrl}api/ShoppingCart/clear`);
  }
}
