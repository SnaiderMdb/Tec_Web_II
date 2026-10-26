import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';
import { Shell } from './layout/shell';

export const routes: Routes = [
  {
    path: 'login',
    title: 'Iniciar sesión · TecWebII',
    loadComponent: () => import('./features/auth/login').then((m) => m.Login),
  },
  {
    path: 'registro',
    title: 'Crear cuenta · TecWebII',
    loadComponent: () => import('./features/auth/registro').then((m) => m.Registro),
  },
  {
    path: '',
    canActivate: [authGuard],
    component: Shell,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      {
        path: 'dashboard',
        title: 'Dashboard · TecWebII',
        loadComponent: () => import('./features/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'productos',
        title: 'Productos · TecWebII',
        loadComponent: () =>
          import('./features/productos/productos-list').then((m) => m.ProductosList),
      },
      {
        path: 'productos/nuevo',
        title: 'Nuevo producto · TecWebII',
        loadComponent: () =>
          import('./features/productos/producto-form').then((m) => m.ProductoForm),
      },
      {
        path: 'productos/:id/editar',
        title: 'Editar producto · TecWebII',
        loadComponent: () =>
          import('./features/productos/producto-form').then((m) => m.ProductoForm),
      },
      {
        path: 'categorias',
        title: 'Categorías · TecWebII',
        loadComponent: () =>
          import('./features/categorias/categorias-list').then((m) => m.CategoriasList),
      },
      {
        path: 'categorias/nueva',
        title: 'Nueva categoría · TecWebII',
        loadComponent: () =>
          import('./features/categorias/categoria-form').then((m) => m.CategoriaForm),
      },
      {
        path: 'categorias/:id/editar',
        title: 'Editar categoría · TecWebII',
        loadComponent: () =>
          import('./features/categorias/categoria-form').then((m) => m.CategoriaForm),
      },
    ],
  },
  { path: '**', redirectTo: '' },
];
