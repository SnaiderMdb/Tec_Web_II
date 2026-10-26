import { CurrencyPipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, finalize } from 'rxjs';
import { Categoria } from '../../core/models/categoria';
import { Producto, ProductoFiltros } from '../../core/models/producto';
import { CategoriaService } from '../../core/services/categoria.service';
import { ProductoService } from '../../core/services/producto.service';
import { ToastService } from '../../core/services/toast.service';
import { extractApiError } from '../../core/utils/api-error';
import { HighlightDirective } from '../../shared/directives/highlight.directive';
import { StockStatusPipe } from '../../shared/pipes/stock-status.pipe';

@Component({
  selector: 'app-productos-list',
  imports: [
    ReactiveFormsModule,
    RouterLink,
    CurrencyPipe,
    StockStatusPipe,
    HighlightDirective,
  ],
  templateUrl: './productos-list.html',
  styleUrl: './productos-list.scss',
})
export class ProductosList {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly productoService = inject(ProductoService);
  private readonly categoriaService = inject(CategoriaService);
  private readonly toastService = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly filtros = this.formBuilder.group({
    search: [''],
    categoriaId: [0],
    activo: ['todos'],
    sortBy: ['nombre'],
    sortDir: ['asc'],
  });

  protected readonly productos = signal<Producto[]>([]);
  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly page = signal(1);
  protected readonly pageSize = 8;
  protected readonly totalItems = signal(0);
  protected readonly totalPages = signal(1);

  protected readonly paginas = computed(() => {
    const total = this.totalPages();
    const actual = this.page();
    const inicio = Math.max(1, Math.min(actual - 2, total - 4));
    const fin = Math.min(total, inicio + 4);

    return Array.from({ length: Math.max(fin - inicio + 1, 0) }, (_, index) => inicio + index);
  });

  constructor() {
    this.cargarCategorias();

    this.filtros.valueChanges
      .pipe(
        debounceTime(300),
        distinctUntilChanged((anterior, actual) => JSON.stringify(anterior) === JSON.stringify(actual)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe(() => {
        this.page.set(1);
        this.cargarProductos();
      });

    this.cargarProductos();
  }

  protected cargarProductos(): void {
    this.loading.set(true);
    this.error.set(null);

    const valores = this.filtros.getRawValue();
    const filtros: ProductoFiltros = {
      search: valores.search,
      categoriaId: valores.categoriaId || null,
      activo: valores.activo === 'todos' ? null : valores.activo === 'activos',
      page: this.page(),
      pageSize: this.pageSize,
      sortBy: valores.sortBy,
      sortDir: valores.sortDir as 'asc' | 'desc',
    };

    this.productoService
      .getPaged(filtros)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (resultado) => {
          this.productos.set(resultado.items);
          this.totalItems.set(resultado.totalItems);
          this.totalPages.set(resultado.totalPages);
        },
        error: (error: unknown) => {
          this.productos.set([]);
          this.error.set(extractApiError(error, 'No fue posible cargar los productos.'));
        },
      });
  }

  protected ordenar(campo: string): void {
    const valores = this.filtros.getRawValue();
    const direccion =
      valores.sortBy === campo && valores.sortDir === 'asc' ? 'desc' : 'asc';

    this.filtros.patchValue({ sortBy: campo, sortDir: direccion });
  }

  protected iconoOrden(campo: string): string {
    const valores = this.filtros.getRawValue();
    if (valores.sortBy !== campo) {
      return '↕';
    }

    return valores.sortDir === 'asc' ? '▲' : '▼';
  }

  protected irAPagina(pagina: number): void {
    if (pagina < 1 || pagina > this.totalPages() || pagina === this.page()) {
      return;
    }

    this.page.set(pagina);
    this.cargarProductos();
  }

  protected limpiarFiltros(): void {
    this.filtros.reset({
      search: '',
      categoriaId: 0,
      activo: 'todos',
      sortBy: 'nombre',
      sortDir: 'asc',
    });
  }

  protected eliminar(producto: Producto): void {
    const confirmado = window.confirm(
      `¿Eliminar el producto "${producto.nombre}"? Esta acción no se puede deshacer.`,
    );

    if (!confirmado) {
      return;
    }

    this.productoService
      .delete(producto.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toastService.success('Producto eliminado correctamente.');

          if (this.productos().length === 1 && this.page() > 1) {
            this.page.update((pagina) => pagina - 1);
          }

          this.cargarProductos();
        },
        error: (error: unknown) => {
          this.toastService.error(extractApiError(error, 'No fue posible eliminar el producto.'));
        },
      });
  }

  private cargarCategorias(): void {
    this.categoriaService
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (categorias) => this.categorias.set(categorias),
        error: () => this.categorias.set([]),
      });
  }
}
