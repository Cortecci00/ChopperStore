import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Response, SteamInventoryItem } from '../Interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class SteamService {
  private _baseUrl = environment.apiUrl;
  private http = inject(HttpClient);

  getInventory(): Observable<Response<SteamInventoryItem[]>> {
    return this.http.get<Response<SteamInventoryItem[]>>(`${this._baseUrl}api/Steam/inventory`);
  }
}
