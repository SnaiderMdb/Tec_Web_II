export interface Producto {
  id: number;
  nombre: string;
  descripcion: string | null;
  codigo: string;
  precio: number;
  stock: number;
  activo: boolean;
  categoriaId: number;
  categoriaNombre: string;
  fechaCreacion: string;
  fechaActualizacion: string | null;
}

export interface ProductoRequest {
  nombre: string;
  descripcion: string | null;
  codigo: string;
  precio: number;
  stock: number;
  categoriaId: number;
  activo: boolean;
}

export interface ProductoFiltros {
  search?: string;
  categoriaId?: number | null;
  activo?: boolean | null;
  page: number;
  pageSize: number;
  sortBy: string;
  sortDir: 'asc' | 'desc';
}
