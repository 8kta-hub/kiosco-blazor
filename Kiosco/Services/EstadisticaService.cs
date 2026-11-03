using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="IEstadisticaService"/>
public class EstadisticaService : IEstadisticaService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;

    public EstadisticaService(IDbContextFactory<KioscoDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<ResumenEncabezado> ObtenerResumenEncabezadoAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();

        int productosActivos = await contexto.Productos.CountAsync(p => p.Activo);

        int productosStockBajo = await contexto.Productos
            .CountAsync(p => p.Activo && p.StockActual <= p.StockMinimo);

        SesionCaja? sesion = await contexto.SesionesCaja
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.FechaCierre == null);

        decimal? saldoCaja = sesion is null
            ? null
            : await ReglasCaja.CalcularSaldoAsync(contexto, sesion);

        // "Hoy" es el día de Argentina: se convierte a UTC para comparar con las fechas guardadas.
        DateTime hoyLocal = ZonaHoraria.AHoraLocal(DateTime.UtcNow).Date;
        DateTime desdeUtc = ZonaHoraria.AUtc(hoyLocal);
        DateTime hastaUtc = ZonaHoraria.AUtc(hoyLocal.AddDays(1));

        IQueryable<Venta> ventasHoy = contexto.Ventas
            .AsNoTracking()
            .Where(v => v.Fecha >= desdeUtc && v.Fecha < hastaUtc);

        int cantidadVentas = await ventasHoy.CountAsync();
        decimal totalVentas = await ventasHoy.SumAsync(v => v.Total);

        return new ResumenEncabezado(productosActivos, productosStockBajo, saldoCaja, cantidadVentas, totalVentas);
    }

    public async Task<PanelKiosco> ObtenerPanelAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();

        List<Producto> stockBajo = await contexto.Productos
            .AsNoTracking()
            .Where(p => p.Activo && p.StockActual <= p.StockMinimo)
            .OrderBy(p => p.StockActual - p.StockMinimo)
            .ThenBy(p => p.Nombre)
            .ToListAsync();

        // Últimos 7 días contando hoy, con el día de Argentina.
        DateTime hoyLocal = ZonaHoraria.AHoraLocal(DateTime.UtcNow).Date;
        DateTime desdeSemanaUtc = ZonaHoraria.AUtc(hoyLocal.AddDays(-6));

        // GroupBy: unidades y dinero por producto vendido.
        var vendidos = await contexto.VentaItems
            .AsNoTracking()
            .Where(i => i.Venta!.Fecha >= desdeSemanaUtc)
            .GroupBy(i => new { i.ProductoId, i.Producto!.Nombre })
            .OrderByDescending(g => g.Sum(i => i.Cantidad))
            .ThenBy(g => g.Key.Nombre)
            .Take(5)
            .Select(g => new
            {
                g.Key.Nombre,
                Unidades = g.Sum(i => i.Cantidad),
                Total = g.Sum(i => i.Cantidad * i.PrecioUnitario)
            })
            .ToListAsync();

        // GroupBy: productos y unidades en stock por categoría.
        var porCategoria = await contexto.Productos
            .AsNoTracking()
            .Where(p => p.Activo)
            .GroupBy(p => p.Categoria!.Nombre)
            .Select(g => new
            {
                Categoria = g.Key,
                Productos = g.Count(),
                Unidades = g.Sum(p => p.StockActual)
            })
            .OrderBy(x => x.Categoria)
            .ToListAsync();

        return new PanelKiosco
        {
            StockBajo = stockBajo,
            MasVendidos = vendidos
                .Select(v => new ProductoVendido(v.Nombre, v.Unidades, v.Total))
                .ToList(),
            StockPorCategoria = porCategoria
                .Select(c => new StockCategoria(c.Categoria, c.Productos, c.Unidades))
                .ToList()
        };
    }
}