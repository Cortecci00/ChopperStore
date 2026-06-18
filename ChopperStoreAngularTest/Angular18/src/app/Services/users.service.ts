import { Injectable, inject } from '@angular/core';
import { Observable } from 'rxjs';
import {
  ActualizarResponse,
  AuthResponse,
  CrearActualizar,
  EliminarResponse,
  Response,
  User,
  UserResponse,
  UsersResponse,
} from '../Interfaces';
import { HttpClient } from '@angular/common/http';
import { environment } from '../../environments/environment';

export interface UpdateProfileDto {
  name?: string;
  lastname?: string;
  username?: string;
  email?: string;
  phone?: string;
}

@Injectable({
  providedIn: 'root',
})
export class UsersService {
  private _baseUrl = environment.apiUrl;

  private http = inject(HttpClient);
  constructor() {}

  getUsers(): Observable<UsersResponse> {
    return this.http.get<UsersResponse>(
      `${this._baseUrl}api/Users`
    );
  }
  getUser(id: string | number): Observable<UserResponse> {
    return this.http.get<UserResponse>(
      `${this._baseUrl}api/Users/${id}`
    );
  }
  postUser(nuevoUser: CrearActualizar): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this._baseUrl}api/Users`,
      nuevoUser
    );
  }
  putUser(
    id: string | number,
    user: CrearActualizar
  ): Observable<ActualizarResponse> {
    return this.http.put<ActualizarResponse>(
      `${this._baseUrl}api/Users/${id}`,
      user
    );
  }
  updateProfile(id: string | number, dto: UpdateProfileDto): Observable<Response<User>> {
    return this.http.put<Response<User>>(
      `${this._baseUrl}api/Users/${id}`,
      dto
    );
  }
  updatePhoto(id: string | number, photoUrl: string): Observable<Response<User>> {
    return this.http.put<Response<User>>(
      `${this._baseUrl}api/Users/${id}/photo`,
      { photoUrl }
    );
  }
  updateSteamTradeUrl(id: string | number, steamTradeUrl: string): Observable<Response<User>> {
    return this.http.put<Response<User>>(
      `${this._baseUrl}api/Users/${id}/steam-trade-url`,
      { steamTradeUrl }
    );
  }
  deleteUser(id: string | number): Observable<EliminarResponse> {
    return this.http.delete<EliminarResponse>(
      `${this._baseUrl}api/Users/${id}`
    );
  }
  loginUser(data: { email: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this._baseUrl}api/Users/login`,
      data
    );
  }

  register(data: { email: string; username: string; password: string }): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this._baseUrl}api/Users/register`,
      data
    );
  }

  loginWithGoogle(googleToken: string): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(
      `${this._baseUrl}api/Users/google-login`,
      { token: googleToken }
    );
  }
}
