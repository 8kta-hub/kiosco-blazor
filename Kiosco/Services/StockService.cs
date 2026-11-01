using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="IStockService"/>
public class StockService : IStockService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;

    public StockService(IDbContextFactory<KioscoDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task RegistrarEntradaAsync(int productoId, int cantidad, string? observacion)
    {
        ValidarCantidad(cantidad);

        await RegistrarAsync(
            productoId,
            TipoMovimientoStock.Entrada,
            MotivoMovimientoStock.Compra,
            _ => cantidad,
            observacion);
    }

    public async Task RegistrarMermaAsync(int productoId, int cantidad, string? observacion)
    {
        ValidarCantidad(cantidad);

        await RegistrarAsync(
            productoId,
            TipoMovimientoStock.Salida,
            MotivoMovimientoStock.Merma,
            _ => -cantidad,
            observacion);
    }

    public async Task RegistrarAjusteAsync(int productoId, int stockContado, string? observacion)
    {
        if (stockContado < 0)
        {
            throw new ReglaDeNegocioException("El stock contado no puede ser negativo.");
        }

        await RegistrarAsync(
            productoId,
            TipoMovimientoStock.Ajuste,
            MotivoMovimientoStock.Ajuste,
            producto =>
            {
                int variacion = stockContado - producto.StockActual;
                if (variacion == 0)
                {
                    throw new ReglaDeNegocioException(
                        "El stock contado coincide con el stock actual: no hay nada que ajustar.");
                }

                return variacion;
            },
            observacion);
    }

    public async Task<PagedResult<MovimientoStock>> ObtenerHistorialAsync(
        int? productoId,
        TipoMovimientoStock? tipo,
        DateTime? desdeUtc,
        DateTime? hastaUtc,
        int pagina,
        int tamanioPagina)
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

        IQueryable<MovimientoStock> consulta = contexto.MovimientosStock
            .AsNoTracking()
            .Include(m => m.Producto);

        if (productoId.HasValue)
        {
            consulta = consulta.Where(m => m.ProductoId == productoId.Value);
        }

        if (tipo.HasValue)
        {
            consulta = consulta.Where(m => m.Tipo == tipo.Value);
        }

        if (desdeUtc.HasValue)
        {
            consulta = consulta.Where(m => m.Fecha >= desdeUtc.Value);
        }

        if (hastaUtc.HasValue)
        {
            consulta = consulta.Where(m => m.Fecha < hastaUtc.Value);
        }

        int total = await consulta.CountAsync();

        List<MovimientoStock> items = await consulta
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.Id)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return new PagedResult<MovimientoStock>
        {
            Items = items,
            TotalItems = total,
            Pagina = pagina,
            TamanioPagina = tamanioPagina
        };
    }

    private async Task RegistrarAsync(
        int productoId,
        TipoMovimientoStock tipo,
        MotivoMovimientoStock motivo,
        Func<Producto, int> calcularVariacion,
        string? observacion)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();

        Producto producto = await contexto.Productos.FindAsync(productoId)
            ?? throw new ReglaDeNegocioException("El producto no existe.");

        int variacion = calcularVariacion(producto);

        MovimientoStock movimiento = ReglasStock.Aplicar(producto, tipo, motivo, variacion, observacion);
        contexto.MovimientosStock.Add(movimiento);

        // Un solo SaveChanges: el cambio de stock y su movimiento se guardan juntos o no se guarda nada.
        await contexto.SaveChangesAsync();
    }

    private static void ValidarCantidad(int cantidad)
    {
        if (cantidad <= 0)
        {
            throw new ReglaDeNegocioException("La cantidad debe ser mayor que cero.");
        }
    }
}