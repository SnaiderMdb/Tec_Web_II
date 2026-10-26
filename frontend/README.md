# TecWebII · Frontend Angular 22

Single Page Application desarrollada con **Angular 22** que consume la API Web Services
del backend en **.NET 10** (rama `Act_04`).

## Requisitos

- Node.js 22 o superior
- Backend en ejecución en `http://localhost:5080`

## Ejecución

```bash
cd frontend
npm install
npm start
```

La aplicación queda disponible en `http://localhost:4200`.

Credenciales de demostración:

| Correo | Contraseña |
| --- | --- |
| `admin@tecwebii.com` | `Admin123*` |

> La URL de la API se configura en `src/environments/environment.ts`
> (`apiUrl: 'http://localhost:5080/api'`).

## Funcionalidades

- Inicio de sesión y registro con JWT (token persistido en `localStorage`).
- Guard funcional que protege las rutas privadas.
- Interceptor HTTP que agrega el token y cierra la sesión ante un `401`.
- Dashboard con indicadores del catálogo, productos recientes y distribución por categoría.
- CRUD completo de productos con búsqueda, filtros, ordenamiento y paginación.
- CRUD de categorías con validación de eliminación cuando existen productos asociados.
- Formularios reactivos con validaciones equivalentes a los DTOs del backend.
- Notificaciones tipo toast para operaciones exitosas y errores de la API.
- Diseño responsive con sidebar colapsable.

## Estructura

```
src/app/
├── core/
│   ├── guards/           # authGuard
│   ├── interceptors/     # authInterceptor (JWT + manejo de 401)
│   ├── models/           # interfaces de productos, categorías, auth y respuestas
│   ├── services/         # AuthService, ProductoService, CategoriaService, ToastService
│   └── utils/            # extracción de mensajes de error de la API
├── features/
│   ├── auth/             # login y registro
│   ├── categorias/       # listado y formulario de categorías
│   ├── dashboard/        # panel de resumen
│   └── productos/        # listado y formulario de productos
├── layout/               # shell con navegación y topbar
├── shared/
│   ├── components/toast/ # notificaciones
│   ├── directives/       # HighlightDirective
│   └── pipes/            # StockStatusPipe
├── app.config.ts         # providers de router, HTTP e interceptor
├── app.routes.ts         # rutas con lazy loading y protección
└── styles.scss           # sistema de diseño global
```

## Elementos evaluados de Angular

| Concepto | Implementación |
| --- | --- |
| Componentes standalone | Todos los componentes de la aplicación |
| Servicios | `AuthService`, `ProductoService`, `CategoriaService`, `ToastService` |
| Directiva personalizada | `HighlightDirective` (`appHighlight`) |
| Pipe personalizado | `StockStatusPipe` (`stockStatus`) |
| Rutas | Lazy loading por componente, rutas hijas y `authGuard` |
| Formularios | Reactive Forms con validaciones |
| Comunicación HTTP | `HttpClient` + interceptor funcional |
| Estado reactivo | Signals y `computed` |
| Control de flujo | `@if`, `@for`, `@empty`, `@switch` |

## Build de producción

```bash
npm run build
```

El resultado se genera en `dist/frontend/browser`.

## Documentación Flashpost

La colección de la API se encuentra en `../TecWebII.Api/docs/flashpost_collection.json`.
