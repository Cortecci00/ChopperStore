import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable, tap } from 'rxjs';
import { Response, ShoppingCart } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ShoppingCartService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  private _cartCount = new BehaviorSubject<number>(0);
  cartCount$ = this._cartCount.asObservable();

  resetCount() {
    this._cartCount.next(0);
  }

  getMine(): Observable<Response<ShoppingCart>> {
    return this.http
      .get<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/mine`)
      .pipe(tap((r) => this._cartCount.next(r.result.items.length)));
  }

  addItem(dto: { skinId: number; quantity: number }): Observable<Response<ShoppingCart>> {
    return this.http
      .post<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/items`, dto)
      .pipe(tap((r) => this._cartCount.next(r.result.items.length)));
  }

  updateItem(itemId: number, dto: { quantity: number }): Observable<Response<ShoppingCart>> {
    return this.http
      .put<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/items/${itemId}`, dto)
      .pipe(tap((r) => this._cartCount.next(r.result.items.length)));
  }

  removeItem(itemId: number): Observable<Response<ShoppingCart>> {
    return this.http
      .delete<Response<ShoppingCart>>(`${this._baseUrl}api/ShoppingCart/items/${itemId}`)
      .pipe(tap((r) => this._cartCount.next(r.result.items.length)));
  }

  clear(): Observable<Response<string>> {
    return this.http
      .delete<Response<string>>(`${this._baseUrl}api/ShoppingCart/clear`)
      .pipe(tap(() => this._cartCount.next(0)));
  }
}
