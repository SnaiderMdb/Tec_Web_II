import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import { NonNullableFormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { CategoriaRequest } from '../../core/models/categoria';
import { CategoriaService } from '../../core/services/categoria.service';
import { ToastService } from '../../core/services/toast.service';
import { extractApiError } from '../../core/utils/api-error';

@Component({
  selector: 'app-categoria-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './categoria-form.html',
  styleUrl: './categoria-form.scss',
})
export class CategoriaForm {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly categoriaService = inject(CategoriaService);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly toastService = inject(ToastService);
  private readonly destroyRef = inject(DestroyRef);

  protected readonly id = signal<number | null>(null);
  protected readonly editando = computed(() => this.id() !== null);
  protected readonly loading = signal(false);
  protected readonly guardando = signal(false);
  protected readonly error = signal<string | null>(null);

  protected readonly form = this.formBuilder.group({
    nombre: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(80)]],
    descripcion: ['', [Validators.maxLength(250)]],
    activo: [true],
  });

  constructor() {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (!idParam) {
      return;
    }

    const id = Number(idParam);
    if (Number.isNaN(id) || id <= 0) {
      this.toastService.error('Identificador de categoría inválido.');
      void this.router.navigate(['/categorias']);
      return;
    }

    this.id.set(id);
    this.cargarCategoria(id);
  }

  protected guardar(): void {
    this.error.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valores = this.form.getRawValue();
    const payload: CategoriaRequest = {
      nombre: valores.nombre.trim(),
      descripcion: valores.descripcion.trim() || null,
      activo: valores.activo,
    };

    const id = this.id();
    const peticion = id
      ? this.categoriaService.update(id, payload)
      : this.categoriaService.create(payload);

    this.guardando.set(true);

    peticion
      .pipe(
        finalize(() => this.guardando.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.toastService.success(
            id ? 'Categoría actualizada correctamente.' : 'Categoría creada correctamente.',
          );
          void this.router.navigate(['/categorias']);
        },
        error: (error: unknown) => {
          this.error.set(extractApiError(error, 'No fue posible guardar la categoría.'));
        },
      });
  }

  private cargarCategoria(id: number): void {
    this.loading.set(true);

    this.categoriaService
      .getById(id)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (categoria) => {
          this.form.patchValue({
            nombre: categoria.nombre,
            descripcion: categoria.descripcion ?? '',
            activo: categoria.activo,
          });
        },
        error: (error: unknown) => {
          this.toastService.error(extractApiError(error, 'No fue posible cargar la categoría.'));
          void this.router.navigate(['/categorias']);
        },
      });
  }
}
