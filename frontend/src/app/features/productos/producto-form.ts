import { Component, DestroyRef, computed, inject, signal } from '@angular/core';
import { takeUntilDestroyed } from '@angular/core/rxjs-interop';
import {
  NonNullableFormBuilder,
  ReactiveFormsModule,
  Validators,
} from '@angular/forms';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { Categoria } from '../../core/models/categoria';
import { ProductoRequest } from '../../core/models/producto';
import { CategoriaService } from '../../core/services/categoria.service';
import { ProductoService } from '../../core/services/producto.service';
import { ToastService } from '../../core/services/toast.service';
import { extractApiError } from '../../core/utils/api-error';

@Component({
  selector: 'app-producto-form',
  imports: [ReactiveFormsModule, RouterLink],
  templateUrl: './producto-form.html',
  styleUrl: './producto-form.scss',
})
export class ProductoForm {
  private readonly formBuilder = inject(NonNullableFormBuilder);
  private readonly productoService = inject(ProductoService);
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
  protected readonly categorias = signal<Categoria[]>([]);

  protected readonly form = this.formBuilder.group({
    nombre: ['', [Validators.required, Validators.minLength(2), Validators.maxLength(100)]],
    codigo: [
      '',
      [
        Validators.required,
        Validators.minLength(2),
        Validators.maxLength(30),
        Validators.pattern(/^[A-Za-z0-9\-_]+$/),
      ],
    ],
    descripcion: ['', [Validators.maxLength(300)]],
    precio: [0, [Validators.required, Validators.min(0.01)]],
    stock: [0, [Validators.required, Validators.min(0), Validators.max(1000000)]],
    categoriaId: [0, [Validators.required, Validators.min(1)]],
    activo: [true],
  });

  constructor() {
    this.cargarCategorias();

    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      const id = Number(idParam);
      if (Number.isNaN(id) || id <= 0) {
        this.toastService.error('Identificador de producto inválido.');
        void this.router.navigate(['/productos']);
        return;
      }

      this.id.set(id);
      this.cargarProducto(id);
    }
  }

  protected guardar(): void {
    this.error.set(null);

    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const valores = this.form.getRawValue();
    const payload: ProductoRequest = {
      nombre: valores.nombre.trim(),
      codigo: valores.codigo.trim().toUpperCase(),
      descripcion: valores.descripcion.trim() || null,
      precio: Number(valores.precio),
      stock: Number(valores.stock),
      categoriaId: Number(valores.categoriaId),
      activo: valores.activo,
    };

    const id = this.id();
    const peticion = id
      ? this.productoService.update(id, payload)
      : this.productoService.create(payload);

    this.guardando.set(true);

    peticion
      .pipe(
        finalize(() => this.guardando.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: () => {
          this.toastService.success(
            id ? 'Producto actualizado correctamente.' : 'Producto creado correctamente.',
          );
          void this.router.navigate(['/productos']);
        },
        error: (error: unknown) => {
          this.error.set(extractApiError(error, 'No fue posible guardar el producto.'));
        },
      });
  }

  private cargarProducto(id: number): void {
    this.loading.set(true);

    this.productoService
      .getById(id)
      .pipe(
        finalize(() => this.loading.set(false)),
        takeUntilDestroyed(this.destroyRef),
      )
      .subscribe({
        next: (producto) => {
          this.form.patchValue({
            nombre: producto.nombre,
            codigo: producto.codigo,
            descripcion: producto.descripcion ?? '',
            precio: producto.precio,
            stock: producto.stock,
            categoriaId: producto.categoriaId,
            activo: producto.activo,
          });
        },
        error: (error: unknown) => {
          this.toastService.error(extractApiError(error, 'No fue posible cargar el producto.'));
          void this.router.navigate(['/productos']);
        },
      });
  }

  private cargarCategorias(): void {
    this.categoriaService
      .getAll()
      .pipe(takeUntilDestroyed(this.destroyRef))
      .subscribe({
        next: (categorias) => this.categorias.set(categorias),
        error: (error: unknown) => {
          this.error.set(extractApiError(error, 'No fue posible cargar las categorías.'));
        },
      });
  }
}
