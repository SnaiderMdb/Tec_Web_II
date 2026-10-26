import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { Observable, map } from 'rxjs';
import { environment } from '../../../environments/environment';
import { ApiResponse, PagedResult } from '../models/api-response';
import { Producto, ProductoFiltros, ProductoRequest } from '../models/producto';

@Injectable({ providedIn: 'root' })
export class ProductoService {
  private readonly http = inject(HttpClient);
  private readonly baseUrl = `${environment.apiUrl}/productos`;

  getPaged(filtros: ProductoFiltros): Observable<PagedResult<Producto>> {
    let params = new HttpParams()
      .set('page', filtros.page)
      .set('pageSize', filtros.pageSize)
      .set('sortBy', filtros.sortBy)
      .set('sortDir', filtros.sortDir);

    if (filtros.search?.trim()) {
      params = params.set('search', filtros.search.trim());
    }

    if (filtros.categoriaId) {
      params = params.set('categoriaId', filtros.categoriaId);
    }

    if (filtros.activo !== undefined && filtros.activo !== null) {
      params = params.set('activo', filtros.activo);
    }

    return this.http
      .get<ApiResponse<PagedResult<Producto>>>(this.baseUrl, { params })
      .pipe(map((response) => response.data));
  }

  getById(id: number): Observable<Producto> {
    return this.http
      .get<ApiResponse<Producto>>(`${this.baseUrl}/${id}`)
      .pipe(map((response) => response.data));
  }

  create(payload: ProductoRequest): Observable<Producto> {
    return this.http
      .post<ApiResponse<Producto>>(this.baseUrl, payload)
      .pipe(map((response) => response.data));
  }

  update(id: number, payload: ProductoRequest): Observable<Producto> {
    return this.http
      .put<ApiResponse<Producto>>(`${this.baseUrl}/${id}`, payload)
      .pipe(map((response) => response.data));
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.baseUrl}/${id}`);
  }
}
