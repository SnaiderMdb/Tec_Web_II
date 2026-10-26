import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, map, tap } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response';
import { AuthResponse, LoginRequest, RegisterRequest, Usuario } from '../models/auth';

const TOKEN_KEY = 'tecwebii.token';
const USER_KEY = 'tecwebii.usuario';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly http = inject(HttpClient);
  private readonly router = inject(Router);

  private readonly _token = signal<string | null>(this.readToken());
  private readonly _usuario = signal<Usuario | null>(this.readUser());

  readonly token = this._token.asReadonly();
  readonly usuario = this._usuario.asReadonly();
  readonly isAuthenticated = computed(() => Boolean(this._token()));
  readonly isAdmin = computed(() => this._usuario()?.rol === 'Administrador');

  login(credentials: LoginRequest): Observable<AuthResponse> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${environment.apiUrl}/auth/login`, credentials)
      .pipe(
        map((response) => response.data),
        tap((session) => this.persistSession(session)),
      );
  }

  register(payload: RegisterRequest): Observable<AuthResponse> {
    return this.http
      .post<ApiResponse<AuthResponse>>(`${environment.apiUrl}/auth/register`, payload)
      .pipe(
        map((response) => response.data),
        tap((session) => this.persistSession(session)),
      );
  }

  me(): Observable<Usuario> {
    return this.http
      .get<ApiResponse<Usuario>>(`${environment.apiUrl}/auth/me`)
      .pipe(
        map((response) => response.data),
        tap((usuario) => {
          this._usuario.set(usuario);
          localStorage.setItem(USER_KEY, JSON.stringify(usuario));
        }),
      );
  }

  logout(redirect = true): void {
    this._token.set(null);
    this._usuario.set(null);
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);

    if (redirect) {
      void this.router.navigate(['/login']);
    }
  }

  private persistSession(session: AuthResponse): void {
    this._token.set(session.token);
    this._usuario.set(session.usuario);
    localStorage.setItem(TOKEN_KEY, session.token);
    localStorage.setItem(USER_KEY, JSON.stringify(session.usuario));
  }

  private readToken(): string | null {
    return localStorage.getItem(TOKEN_KEY);
  }

  private readUser(): Usuario | null {
    const raw = localStorage.getItem(USER_KEY);
    if (!raw) {
      return null;
    }

    try {
      return JSON.parse(raw) as Usuario;
    } catch {
      localStorage.removeItem(USER_KEY);
      return null;
    }
  }
}
