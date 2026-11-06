using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="IProductoService"/>
public class ProductoService : IProductoService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;
    private readonly ILogger<ProductoService> _logger;

    public ProductoService(IDbContextFactory<KioscoDbContext> fabrica, ILogger<ProductoService> logger)
    {
        _fabrica = fabrica;
        _logger = logger;
    }

    public Task<PagedResult<Producto>> ObtenerPaginadoAsync(string? busqueda, int pagina, int tamanioPagina)
    {
        return _logger.RegistrarLecturaAsync("Listar productos", async () =>
        {
            if (pagina < 1)
            {
                pagina = 1;
            }

            if (tamanioPagina < 1)
            {
                tamanioPagina = 10;
            }

            await using var contexto = await _fabrica.CreateDbContextAsync();

            IQueryable<Producto> consulta = contexto.Productos
                .AsNoTracking()
                .Include(p => p.Categoria);

            if (!string.IsNullOrWhiteSpace(busqueda))
            {
                string texto = busqueda.Trim();
                consulta = consulta.Where(p => p.Nombre.Contains(texto));
            }

            int total = await consulta.CountAsync();

            List<Producto> items = await consulta
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Id)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return new PagedResult<Producto>
            {
                Items = items,
                TotalItems = total,
                Pagina = pagina,
                TamanioPagina = tamanioPagina
            };
        });
    }

    public Task<List<Producto>> ObtenerTodosAsync(bool soloActivos)
    {
        return _logger.RegistrarLecturaAsync("Listar todos los productos", async () =>
        {
            await using var contexto = await _fabrica.CreateDbContextAsync();

            IQueryable<Producto> consulta = contexto.Productos.AsNoTracking();

            if (soloActivos)
            {
                consulta = consulta.Where(p => p.Activo);
            }

            return await consulta
                .OrderBy(p => p.Nombre)
                .ThenBy(p => p.Id)
                .ToListAsync();
        });
    }

    public Task<Producto?> ObtenerPorIdAsync(int id)
    {
        return _logger.RegistrarLecturaAsync($"Buscar producto #{id}", async () =>
        {
            await using var contexto = await _fabrica.CreateDbContextAsync();

            return await contexto.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
        });
    }

    public Task<Producto> CrearAsync(Producto producto)
    {
        return _logger.RegistrarOperacionAsync($"Crear producto «{producto.Nombre}»", async () =>
        {
            Normalizar(producto);
            producto.Id = 0;
            producto.Categoria = null;

            int stockInicial = producto.StockActual;
            if (stockInicial < 0)
            {
                throw new ReglaDeNegocioException("El stock inicial no puede ser negativo.");
            }

            // El stock se carga a través de un movimiento, así el historial siempre coincide con el stock.
            producto.StockActual = 0;

            await using var contexto = await _fabrica.CreateDbContextAsync();

            contexto.Productos.Add(producto);

            if (stockInicial > 0)
            {
                MovimientoStock movimiento = ReglasStock.Aplicar(
                    producto,
                    TipoMovimientoStock.Entrada,
                    MotivoMovimientoStock.Ajuste,
                    stockInicial,
                    "Stock inicial");

                contexto.MovimientosStock.Add(movimiento);
            }

            // Un solo SaveChanges: el producto y su movimiento inicial se guardan juntos.
            await contexto.SaveChangesAsync();

            return producto;
        });
    }

    public Task ActualizarAsync(Producto producto)
    {
        return _logger.RegistrarOperacionAsync($"Actualizar producto #{producto.Id}", async () =>
        {
            Normalizar(producto);

            await using var contexto = await _fabrica.CreateDbContextAsync();

            Producto existente = await contexto.Productos.FindAsync(producto.Id)
                ?? throw new InvalidOperationException("El producto ya no existe.");

            // Se copian solo los campos editables: el stock actual no se toca desde el formulario.
            existente.Nombre = producto.Nombre;
            existente.CodigoBarras = producto.CodigoBarras;
            existente.PrecioCosto = producto.PrecioCosto;
            existente.PrecioVenta = producto.PrecioVenta;
            existente.StockMinimo = producto.StockMinimo;
            existente.Activo = producto.Activo;
            existente.CategoriaId = producto.CategoriaId;

            await contexto.SaveChangesAsync();
        });
    }

    public Task EliminarAsync(int id)
    {
        return _logger.RegistrarOperacionAsync($"Eliminar producto #{id}", async () =>
        {
            await using var contexto = await _fabrica.CreateDbContextAsync();

            Producto? producto = await contexto.Productos.FindAsync(id);
            if (producto is null)
            {
                return;
            }

            bool tieneMovimientos = await contexto.MovimientosStock.AnyAsync(m => m.ProductoId == id);
            if (tieneMovimientos)
            {
                throw new ReglaDeNegocioException(
                    "No se puede eliminar un producto con movimientos de stock. Para dejar de usarlo, desmarcá «Producto activo» al editarlo.");
            }

            contexto.Productos.Remove(producto);
            await contexto.SaveChangesAsync();
        });
    }

    private static void Normalizar(Producto producto)
    {
        producto.Nombre = producto.Nombre.Trim();
        producto.CodigoBarras = string.IsNullOrWhiteSpace(producto.CodigoBarras)
            ? null
            : producto.CodigoBarras.Trim();
    }
}