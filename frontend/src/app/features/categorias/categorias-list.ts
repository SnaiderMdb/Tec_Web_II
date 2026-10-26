import { DatePipe } from '@angular/common';
import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Categoria } from '../../core/models/categoria';
import { CategoriaService } from '../../core/services/categoria.service';
import { ToastService } from '../../core/services/toast.service';
import { extractApiError } from '../../core/utils/api-error';
import { HighlightDirective } from '../../shared/directives/highlight.directive';

@Component({
  selector: 'app-categorias-list',
  imports: [RouterLink, DatePipe, HighlightDirective],
  templateUrl: './categorias-list.html',
  styleUrl: './categorias-list.scss',
})
export class CategoriasList {
  private readonly categoriaService = inject(CategoriaService);
  private readonly toastService = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly categorias = signal<Categoria[]>([]);
  protected readonly loading = signal(true);
  protected readonly error = signal<string | null>(null);
  protected readonly filtro = signal('');

  protected readonly categoriasFiltradas = computed(() => {
    const termino = this.filtro().trim().toLowerCase();
    if (!termino) {
      return this.categorias();
    }

    return this.categorias().filter(
      (categoria) =>
        categoria.nombre.toLowerCase().includes(termino) ||
        categoria.descripcion?.toLowerCase().includes(termino),
    );
  });

  protected readonly totalProductos = computed(() =>
    this.categorias().reduce((total, categoria) => total + categoria.cantidadProductos, 0),
  );

  constructor() {
    this.cargarCategorias();
  }

  protected cargarCategorias(): void {
    this.loading.set(true);
    this.error.set(null);

    this.categoriaService
      .getAll()
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (categorias) => this.categorias.set(categorias),
        error: (error: unknown) => {
          this.error.set(extractApiError(error, 'No fue posible cargar las categorías.'));
        },
      });
  }

  protected actualizarFiltro(event: Event): void {
    const input = event.target as HTMLInputElement;
    this.filtro.set(input.value);
  }

  protected eliminar(categoria: Categoria): void {
    const confirmado = window.confirm(
      `¿Eliminar la categoría "${categoria.nombre}"? Solo es posible si no tiene productos asociados.`,
    );

    if (!confirmado) {
      return;
    }

    this.categoriaService
      .delete(categoria.id)
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: () => {
          this.toastService.success('Categoría eliminada correctamente.');
          this.cargarCategorias();
        },
        error: (error: unknown) => {
          this.toastService.error(extractApiError(error, 'No fue posible eliminar la categoría.'));
        },
      });
  }
}
