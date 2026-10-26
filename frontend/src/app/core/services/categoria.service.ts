import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse } from '../models/api-response';
import { Categoria, CategoriaRequest } from '../models/categoria';

@Injectable({ providedIn: 'root' })
export class CategoriaService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/categorias`;

  getAll(activo?: boolean): Observable<Categoria[]> {
    const url = activo === undefined ? this.baseUrl : `${this.baseUrl}?activo=${activo}`;
    return this.http
      .get<ApiResponse<Categoria[]>>(url)
      .pipe(map((response) => response.data));
  }

  getById(id: number): Observable<Categoria> {
    return this.http
      .get<ApiResponse<Categoria>>(`${this.baseUrl}/${id}`)
      .pipe(map((response) => response.data));
  }

  create(payload: CategoriaRequest): Observable<Categoria> {
    return this.http
      .post<ApiResponse<Categoria>>(this.baseUrl, payload)
      .pipe(map((response) => response.data));
  }

  update(id: number, payload: CategoriaRequest): Observable<Categoria> {
    return this.http
      .put<ApiResponse<Categoria>>(`${this.baseUrl}/${id}`, payload)
      .pipe(map((response) => response.data));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
