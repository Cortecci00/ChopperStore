import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ContactMessage, ContactMessageCreate, Response } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class ContactService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  create(dto: ContactMessageCreate): Observable<Response<ContactMessage>> {
    return this.http.post<Response<ContactMessage>>(`${this._baseUrl}api/Contact`, dto);
  }

  getAll(): Observable<Response<ContactMessage[]>> {
    return this.http.get<Response<ContactMessage[]>>(`${this._baseUrl}api/Contact`);
  }
}
