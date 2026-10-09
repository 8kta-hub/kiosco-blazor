using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Data;

/// <summary>
/// Contexto de Entity Framework Core del kiosco: conecta las entidades con SQL Server.
/// </summary>
public class KioscoDbContext : DbContext
{
    public KioscoDbContext(DbContextOptions<KioscoDbContext> opciones)
        : base(opciones)
    {
    }

    public DbSet<Categoria> Categorias => Set<Categoria>();

    public DbSet<Producto> Productos => Set<Producto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Categoria>(entidad =>
        {
            entidad.HasIndex(c => c.Nombre).IsUnique();
        });

        modelBuilder.Entity<Producto>(entidad =>
        {
            entidad.Property(p => p.PrecioCosto).HasPrecision(18, 2);
            entidad.Property(p => p.PrecioVenta).HasPrecision(18, 2);

            entidad.HasIndex(p => p.Nombre);

            entidad.HasOne(p => p.Categoria)
                   .WithMany(c => c.Productos)
                   .HasForeignKey(p => p.CategoriaId)
                   .OnDelete(DeleteBehavior.Restrict);
        });

        SembrarDatos(modelBuilder);
    }

    private static void SembrarDatos(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Categoria>().HasData(
            new Categoria { Id = 1, Nombre = "Bebidas" },
            new Categoria { Id = 2, Nombre = "Golosinas" },
            new Categoria { Id = 3, Nombre = "Snacks" },
            new Categoria { Id = 4, Nombre = "Galletitas" },
            new Categoria { Id = 5, Nombre = "Varios" });

        modelBuilder.Entity<Producto>().HasData(
            new Producto { Id = 1, Nombre = "Coca-Cola 500 ml", CategoriaId = 1, PrecioCosto = 1100m, PrecioVenta = 1600m, StockActual = 24, StockMinimo = 12 },
            new Producto { Id = 2, Nombre = "Agua mineral 500 ml", CategoriaId = 1, PrecioCosto = 600m, PrecioVenta = 900m, StockActual = 4, StockMinimo = 12 },
            new Producto { Id = 3, Nombre = "Jugo en caja 200 ml", CategoriaId = 1, PrecioCosto = 500m, PrecioVenta = 800m, StockActual = 18, StockMinimo = 8 },
            new Producto { Id = 4, Nombre = "Alfajor triple", CategoriaId = 2, PrecioCosto = 700m, PrecioVenta = 1200m, StockActual = 2, StockMinimo = 10 },
            new Producto { Id = 5, Nombre = "Chicles menta", CategoriaId = 2, PrecioCosto = 350m, PrecioVenta = 600m, StockActual = 1, StockMinimo = 6 },
            new Producto { Id = 6, Nombre = "Caramelos masticables", CategoriaId = 2, PrecioCosto = 200m, PrecioVenta = 350m, StockActual = 40, StockMinimo = 15 },
            new Producto { Id = 7, Nombre = "Papas fritas 80 g", CategoriaId = 3, PrecioCosto = 1300m, PrecioVenta = 1900m, StockActual = 15, StockMinimo = 6 },
            new Producto { Id = 8, Nombre = "Maní salado 100 g", CategoriaId = 3, PrecioCosto = 800m, PrecioVenta = 1200m, StockActual = 10, StockMinimo = 5 },
            new Producto { Id = 9, Nombre = "Galletitas de agua", CategoriaId = 4, PrecioCosto = 900m, PrecioVenta = 1400m, StockActual = 20, StockMinimo = 8 },
            new Producto { Id = 10, Nombre = "Pilas AA x2", CategoriaId = 5, PrecioCosto = 1500m, PrecioVenta = 2300m, StockActual = 8, StockMinimo = 3 });
    }
}