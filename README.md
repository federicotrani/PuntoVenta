# PuntoVenta

Sistema de punto de venta desarrollado con .NET 10, Blazor WebAssembly, ASP.NET Core Web API y Entity Framework Core.

## Descripción

La solución separa la aplicación en tres proyectos:

- **PuntoVenta.Client**: interfaz web desarrollada con Blazor WebAssembly.
- **PuntoVenta.Server**: backend ASP.NET Core Web API, acceso a datos y generación de reportes.
- **PuntoVenta.Shared**: entidades, DTOs y enumeraciones compartidas entre el cliente y el servidor.

Actualmente la aplicación incluye la gestión de categorías y productos. El menú de usuario y la sesión son mock y no representan todavía un sistema de autenticación real.

## Tecnologías

- .NET 10
- C# 14
- Blazor WebAssembly
- ASP.NET Core Web API
- Entity Framework Core 10
- SQL Server / SQL Server Express
- Serilog con salida a consola
- Bootstrap
- QuestPDF
- BCrypt.Net-Next

## Estructura de la solución

```text
PuntoVenta/
├── PuntoVenta.slnx
├── PuntoVenta.Client/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   ├── NavMenu.razor
│   │   └── UserMenu.razor
│   ├── Pages/
│   │   ├── Categorias/
│   │   │   ├── CategoriaDetalle.razor
│   │   │   └── CategoriaLista.razor
│   │   ├── Productos/
│   │   │   └── ProductoLista.razor
│   │   └── Home.razor
│   ├── Services/
│   │   ├── CategoriaService.cs
│   │   ├── ICategoriaService.cs
│   │   ├── ProductoService.cs
│   │   └── IProductoService.cs
│   └── wwwroot/
├── PuntoVenta.Server/
│   ├── Controllers/
│   │   ├── CategoriasController.cs
│   │   ├── ProductosController.cs
│   │   ├── ReportesController.cs
│   │   └── UsuariosController.cs
│   ├── Data/
│   │   └── DataContext.cs
│   ├── Migrations/
│   ├── Services/
│   │   └── CategoriaPdfService.cs
│   ├── Program.cs
│   ├── appsettings.json
│   └── PuntoVenta.Server.http
└── PuntoVenta.Shared/
    ├── DTOs/
    ├── Entities/
    └── Enums/
```

## Requisitos

- .NET 10 SDK
- SQL Server o SQL Server Express
- Visual Studio 2026, Visual Studio Code o Rider
- Herramientas de Entity Framework Core, si se van a administrar migraciones:

```powershell
dotnet tool install --global dotnet-ef
```

## Configuración de la base de datos

La cadena de conexión se encuentra en `PuntoVenta.Server/appsettings.json`.

Configuración actual por defecto:

```text
Server=.\sqlexpress;Database=PuntoVentaDB;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true;
```

Ajuste la cadena de conexión según la instalación local de SQL Server. Para aplicar las migraciones existentes:

```powershell
dotnet ef database update --project PuntoVenta.Server
```

## Ejecución

Restaure las dependencias y compile la solución:

```powershell
dotnet restore PuntoVenta.slnx
dotnet build PuntoVenta.slnx
```

Inicie el backend en una terminal:

```powershell
dotnet run --project PuntoVenta.Server
```

El servidor está configurado para ejecutarse en `http://localhost:5033`.

Inicie el cliente en otra terminal:

```powershell
dotnet run --project PuntoVenta.Client
```

El cliente utiliza `http://localhost:5033` como dirección base para las llamadas a la API. El backend tiene CORS habilitado para permitir el desarrollo local.

## API disponible

### Categorías

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/Categorias` | Obtiene todas las categorías. |
| `GET` | `/api/Categorias/{id}` | Obtiene una categoría por ID. |
| `POST` | `/api/Categorias` | Crea una categoría. |
| `PUT` | `/api/Categorias/{id}` | Actualiza una categoría. |
| `DELETE` | `/api/Categorias/{id}` | Elimina una categoría. |
| `GET` | `/api/Categorias/pdf` | Descarga el listado de categorías en PDF. |

Ejemplo de creación o actualización:

```json
{
  "nombre": "Bebidas",
  "descripcion": "Productos líquidos y bebidas"
}
```

### Productos

| Método | Ruta | Descripción |
|---|---|---|
| `GET` | `/api/Productos` | Obtiene todos los productos. |
| `GET` | `/api/Productos/{id}` | Obtiene un producto por ID. |
| `POST` | `/api/Productos` | Crea un producto. |
| `PUT` | `/api/Productos/{id}` | Actualiza un producto. |
| `DELETE` | `/api/Productos/{id}` | Elimina un producto. |

Los controladores de usuarios y reportes están incluidos como estructura inicial y todavía no exponen operaciones funcionales.

## Logging

El backend utiliza Serilog. Los eventos se escriben en la consola mediante `Serilog.Sinks.Console`.

- Nivel predeterminado de la aplicación: `Information`.
- Componentes de Microsoft y Entity Framework Core: `Warning`.
- Los errores registrados mediante `ILogger` aparecen en la consola del backend.

La configuración se encuentra en `PuntoVenta.Server/appsettings.json`.

## Estado actual

- Gestión de categorías: listado, alta, edición, eliminación con confirmación y exportación a PDF.
- Gestión de productos: endpoints y listado inicial.
- Menú de usuario: disponible con usuario mock.
- Autenticación y autorización: pendientes de implementación.
- Configuración de usuario: pendiente de implementación.
- Gestión de usuarios y ventas: estructura inicial pendiente de completar.

## Licencia

Este proyecto no define actualmente una licencia de distribución.