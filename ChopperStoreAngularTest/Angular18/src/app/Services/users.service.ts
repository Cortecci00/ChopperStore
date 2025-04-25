import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActualizarResponse,
  CrearActualizar,
  CrearResponse,
  EliminarResponse,
  UserResponse,
  UsersResponse,
} from '../Interfaces';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  // private _baseUrl = 'https://localhost:7019/';
  private _baseUrl = 'http://localhost:5036/';

  private http = inject(HttpClient);
  constructor() {}

  getUsers(): Observable<UsersResponse> {
    return this.http.get<UsersResponse>(
      `${this._baseUrl}api/Users`
    );
  }
  getUser(id: string): Observable<UserResponse> {
    return this.http.get<UserResponse>(
      `${this._baseUrl}api/Users/${id}`
    );
  }
  postUser(nuevoUser: CrearActualizar): Observable<CrearResponse> {
    return this.http.post<CrearResponse>(
      `${this._baseUrl}api/Users`,
      nuevoUser
    );
  }
  putUser(
    id: string,
    user: CrearActualizar
  ): Observable<ActualizarResponse> {
    return this.http.put<ActualizarResponse>(
      `${this._baseUrl}api/Users/${id}`,
      user
    );
  }
  deleteUser(id: string): Observable<EliminarResponse> {
    return this.http.delete<EliminarResponse>(
      `${this._baseUrl}api/Users/${id}`
    );
  }
  loginUser(data: { email: string; password: string }) {
    return this.http.post<{ isSuccess: boolean; result: any }>(
      `${this._baseUrl}api/Users/login`,
      data
    );
  }

  loginWithGoogle(googleToken: string) {
    return this.http.post<{ isSuccess: boolean; result: any }>(
      `${this._baseUrl}api/Users/google-login`,
      { token: googleToken }
    );
  }
}
