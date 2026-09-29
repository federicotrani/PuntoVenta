# Modelo relacional - PuntoVenta

Generado a partir de `PuntoVenta.Shared/Entities`.

```mermaid
erDiagram
    CATEGORIA {
        int Id PK
        string Nombre "MaxLength 100"
        string Descripcion "MaxLength 300"
    }

    PRODUCTO {
        int Id PK
        string Nombre "MaxLength 200"
        decimal Precio "decimal(18,2)"
        int StockActual
        int StockMinimo
        string UrlImagen "MaxLength 300, nullable"
        bool Activo
        int CategoriaId FK
    }

    USUARIO {
        int Id PK
        string NombreCompleto "MaxLength 200"
        string NombreUsuario "MaxLength 50"
        string Email "MaxLength 200"
        string HashPassword "MaxLength 100"
        datetime FechaAlta
        bool Activo
        int Rol "enum RolUsuario"
    }

    VENTA {
        int Id PK
        datetime Fecha
        decimal MontoTotal "decimal(18,2)"
        int UsuarioId FK
        int Estado "enum EstadoVenta"
        int FormaPago "enum FormaPago"
        string Notas "MaxLength 1000, nullable"
    }

    VENTA_DETALLE {
        int Id PK
        int VentaId FK
        int ProductoId FK
        int Cantidad
        decimal PrecioUnitario "decimal(18,2)"
    }

    STOCK {
        int Id PK
        datetime Fecha
        int TipoMovimiento "enum TipoMovimiento"
        int ProductoId FK
        int Cantidad
        int UsuarioId FK
        string Notas "MaxLength 1000, nullable"
    }

    CATEGORIA ||--o{ PRODUCTO : "clasifica"
    USUARIO   ||--o{ VENTA : "registra"
    USUARIO   ||--o{ STOCK : "realiza"
    VENTA     ||--|{ VENTA_DETALLE : "contiene"
    PRODUCTO  ||--o{ VENTA_DETALLE : "se vende en"
    PRODUCTO  ||--o{ STOCK : "tiene movimientos"
```

## Notas

- `VentaDetalle.MontoTotal` es una propiedad calculada (`Cantidad * PrecioUnitario`) que solo tiene getter. EF Core no la mapea a una columna, por eso no aparece en el diagrama.
- `VentaDetalle` tiene `VentaId` como FK, pero le falta la propiedad de navegación `Venta`. La relación existe igualmente por el `[ForeignKey("Venta")]` y por la colección en `Venta`.
- Los enums (`RolUsuario`, `EstadoVenta`, `FormaPago`, `TipoMovimiento`) se guardan como `int` en la base de datos.
