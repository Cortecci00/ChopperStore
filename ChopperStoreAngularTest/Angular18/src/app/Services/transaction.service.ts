import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CheckoutResult, Response, Transaction } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class TransactionService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  checkout(): Observable<Response<CheckoutResult>> {
    return this.http.post<Response<CheckoutResult>>(`${this._baseUrl}api/Transaction/checkout`, {});
  }

  getMine(): Observable<Response<Transaction[]>> {
    return this.http.get<Response<Transaction[]>>(`${this._baseUrl}api/Transaction/mine`);
  }

  getById(id: number): Observable<Response<Transaction>> {
    return this.http.get<Response<Transaction>>(`${this._baseUrl}api/Transaction/${id}`);
  }

  getAll(): Observable<Response<Transaction[]>> {
    return this.http.get<Response<Transaction[]>>(`${this._baseUrl}api/Transaction`);
  }
}
