import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Recommendation, Response } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class RecommendationService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getAll(): Observable<Response<Recommendation[]>> {
    return this.http.get<Response<Recommendation[]>>(`${this._baseUrl}api/Recommendations`);
  }

  create(text: string): Observable<Response<Recommendation>> {
    return this.http.post<Response<Recommendation>>(`${this._baseUrl}api/Recommendations`, { text });
  }

  delete(id: number): Observable<Response<Recommendation>> {
    return this.http.delete<Response<Recommendation>>(`${this._baseUrl}api/Recommendations/${id}`);
  }
}
