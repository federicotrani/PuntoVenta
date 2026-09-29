using Microsoft.EntityFrameworkCore;
using PuntoVenta.Shared.Entities;

namespace PuntoVenta.Server.Data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<Categoria> Categorias { get; set; }
    public DbSet<Producto> Productos { get; set; }
    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Venta> Ventas { get; set; }
    public DbSet<VentaDetalle> VentaDetalles { get; set; }
    public DbSet<Stock> Stocks { get; set; }

}
