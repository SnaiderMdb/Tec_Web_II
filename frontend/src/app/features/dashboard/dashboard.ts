import { CurrencyPipe, DatePipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { forkJoin, finalize } from 'rxjs';
import { Categoria } from '../../core/models/categoria';
import { Producto } from '../../core/models/producto';
import { CategoriaService } from '../../core/services/categoria.service';
import { ProductoService } from '../../core/services/producto.service';
import { extractApiError } from '../../core/utils/api-error';
import { StockStatusPipe } from '../../shared/pipes/stock-status.pipe';

@Component({
  selector: 'app-dashboard',
  imports: [RouterLink, CurrencyPipe, DatePipe, StockStatusPipe],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.scss',
})
export class Dashboard {
  private readonly productoService = inject(ProductoService);
  private readonly categoriaService = inject(CategoriaService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly productos = signal<Producto[]>([]);
  protected readonly categorias = signal<Categoria[]>([]);

  protected readonly totalProductos = computed(() => this.productos().length);
  protected readonly productosActivos = computed(
    () => this.productos().filter((producto) => producto.activo).length,
  );
  protected readonly productosStockBajo = computed(
    () => this.productos().filter((producto) => producto.activo && producto.stock <= 5).length,
  );
  protected readonly valorInventario = computed(() =>
    this.productos().reduce((total, producto) => total + producto.precio * producto.stock, 0),
  );
  protected readonly categoriasActivas = computed(
    () => this.categorias().filter((categoria) => categoria.activo).length,
  );
  protected readonly ultimosProductos = computed(() => this.productos().slice(0, 5));
  protected readonly topCategorias = computed(() => {
    const total = this.totalProductos();

    return [...this.categorias()]
      .sort((a, b) => b.cantidadProductos - a.cantidadProductos)
      .slice(0, 4)
      .map((categoria) => ({
        ...categoria,
        porcentaje: total === 0 ? 0 : Math.round((categoria.cantidadProductos / total) * 100),
      }));
  });

  constructor() {
    this.load();
  }

  private load(): void {
    this.loading.set(true);
    this.error.set(null);

    forkJoin({
      productos: this.productoService.getPaged({
        page: 1,
        pageSize: 100,
        sortBy: 'fecha',
        sortDir: 'desc',
      }),
      categorias: this.categoriaService.getAll(),
    })
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: ({ productos, categorias }) => {
          this.productos.set(productos.items);
          this.categorias.set(categorias);
        },
        error: (error: unknown) => {
          this.error.set(extractApiError(error, 'No fue posible cargar el dashboard.'));
        },
      });
  }

  protected reload(): void {
    this.load();
  }
}
