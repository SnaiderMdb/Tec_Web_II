# Tec_Web_II

Repositorio de actividades de la asignatura **Tecnologías Web II**.

| Rama | Actividad | Contenido |
| --- | --- | --- |
| `Act_04` | Programación lógica del Backend, autenticación, middlewares y API Web Services | API REST en .NET 10 (Entity Framework Core, JWT, DTOs, Swagger) |
| `Act_05` | Desarrollo de SPA con framework JS | Backend de la Actividad 4 + frontend Angular 22 |

## Estructura del repositorio (rama `Act_04`)

```
Tec_Web_II/
├── TecWebII.slnx
├── TecWebII.Api/                 # Proyecto Web API (.NET 10)
│   ├── controllers/              # Endpoints REST
│   ├── core/                     # DbContext, respuestas, middlewares, seguridad, migraciones
│   ├── dtos/                     # Data Transfer Objects con DataAnnotations
│   ├── models/                   # Entidades Producto y Categoría
│   ├── services/                 # Interfaces e implementaciones de la lógica de negocio
│   ├── docs/flashpost_collection.json
│   ├── Program.cs
│   └── appsettings.json
└── README.md
```

La rama `Act_05` conserva el backend y agrega el proyecto `frontend/` (Angular 22).

## Requisitos

- [.NET SDK 10](https://dotnet.microsoft.com/download)
- [Node.js 22+](https://nodejs.org/) (solo para el frontend de la Actividad 5)

## Ejecución del backend (rama `Act_04`)

```bash
cd TecWebII.Api
dotnet restore
dotnet run
```

La API queda disponible en `http://localhost:5080` y la documentación interactiva en
`http://localhost:5080/swagger`.

La base de datos SQLite (`tecwebii.db`) y las migraciones se aplican automáticamente al
iniciar. También se cargan datos de demostración:

| Correo | Contraseña | Rol |
| --- | --- | --- |
| `admin@tecwebii.com` | `Admin123*` | Administrador |

## Endpoints principales

| Método | Ruta | Autenticación | Descripción |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | Pública | Registro de usuario, devuelve JWT |
| POST | `/api/auth/login` | Pública | Inicio de sesión, devuelve JWT |
| GET | `/api/auth/me` | JWT | Perfil del usuario autenticado |
| GET | `/api/categorias` | Pública | Listado de categorías |
| GET | `/api/categorias/{id}` | Pública | Detalle de categoría |
| POST | `/api/categorias` | JWT | Crear categoría |
| PUT | `/api/categorias/{id}` | JWT | Actualizar categoría |
| DELETE | `/api/categorias/{id}` | JWT | Eliminar categoría sin productos |
| GET | `/api/productos` | Pública | Listado paginado con filtros |
| GET | `/api/productos/{id}` | Pública | Detalle de producto |
| POST | `/api/productos` | JWT | Crear producto |
| PUT | `/api/productos/{id}` | JWT | Actualizar producto |
| DELETE | `/api/productos/{id}` | JWT | Eliminar producto |

### Filtros del listado de productos

`GET /api/productos?search=laptop&categoriaId=1&activo=true&precioMin=100&precioMax=1000&page=1&pageSize=10&sortBy=precio&sortDir=asc`

## Documentación Flashpost

La colección de peticiones lista para importar en la extensión **Flashpost** está en:

```
TecWebII.Api/docs/flashpost_collection.json
```

Incluye el login, el registro, el perfil y el CRUD completo de categorías y productos.
Después de ejecutar el login, copie el token en la variable de entorno `token` de la
colección para autenticar las peticiones protegidas.

## Frontend Angular (rama `Act_05`)

La rama `Act_05` agrega el proyecto `frontend/`, una SPA en **Angular 22** que consume
esta misma API.

```bash
cd frontend
npm install
npm start
```

La aplicación queda disponible en `http://localhost:4200` y requiere que el backend esté
escuchando en `http://localhost:5080`. Credenciales: `admin@tecwebii.com` / `Admin123*`.

El frontend incluye:

- Componentes standalone con lazy loading y rutas protegidas por `authGuard`.
- Servicios para autenticación, productos, categorías y notificaciones.
- Interceptor funcional que agrega el token JWT y maneja respuestas `401`.
- Directiva personalizada `appHighlight` y pipe `stockStatus`.
- Formularios reactivos con validaciones, dashboard, filtros, ordenamiento y paginación.


## Buenas prácticas aplicadas

- Separación en capas: `models`, `dtos`, `core`, `services` y `controllers`.
- DTOs con **DataAnnotations** para validación automática y mensajes en español.
- **Entity Framework Core** con SQLite, migraciones y datos semilla.
- Autenticación con **JWT** y hashing de contraseñas **PBKDF2-SHA256**.
- Middlewares globales de manejo de errores y logging de peticiones.
- Respuestas uniformes mediante `ApiResponse<T>` y paginación con `PagedResult<T>`.
- Documentación OpenAPI/Swagger con soporte para token Bearer.
- CORS configurable para el cliente Angular.
