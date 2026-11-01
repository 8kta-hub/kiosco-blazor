using Kiosco.Data;
using Kiosco.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="ICajaService"/>
public class CajaService : ICajaService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;

    public CajaService(IDbContextFactory<KioscoDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<ResumenCaja?> ObtenerResumenAbiertaAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();

        SesionCaja? sesion = await contexto.SesionesCaja
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.FechaCierre == null);

        if (sesion is null)
        {
            return null;
        }

        (decimal ingresos, decimal egresos) = await ReglasCaja.ObtenerTotalesAsync(contexto, sesion.Id);
        return new ResumenCaja(sesion, ingresos, egresos);
    }

    public async Task<SesionCaja> AbrirAsync(decimal montoInicial)
    {
        ReglasCaja.ValidarMonto(montoInicial, permitirCero: true);

        await using var contexto = await _fabrica.CreateDbContextAsync();

        bool yaHayCajaAbierta = await contexto.SesionesCaja.AnyAsync(s => s.FechaCierre == null);
        if (yaHayCajaAbierta)
        {
            throw new ReglaDeNegocioException("Ya hay una caja abierta.");
        }

        var sesion = new SesionCaja
        {
            FechaApertura = DateTime.UtcNow,
            MontoInicial = montoInicial
        };

        contexto.SesionesCaja.Add(sesion);

        try
        {
            await contexto.SaveChangesAsync();
        }
        catch (DbUpdateException ex) when (EsViolacionDeUnicidad(ex))
        {
            // Otra operación abrió la caja justo antes: el índice único de la base lo impidió.
            throw new ReglaDeNegocioException("Ya hay una caja abierta.");
        }

        return sesion;
    }

    public async Task RegistrarIngresoAsync(string concepto, decimal monto)
    {
        await RegistrarMovimientoAsync(TipoMovimientoCaja.Ingreso, concepto, monto);
    }

    public async Task RegistrarEgresoAsync(string concepto, decimal monto)
    {
        await RegistrarMovimientoAsync(TipoMovimientoCaja.Egreso, concepto, monto);
    }

    public async Task<ResumenCaja> CerrarAsync(decimal efectivoContado)
    {
        ReglasCaja.ValidarMonto(efectivoContado, permitirCero: true);

        await using var contexto = await _fabrica.CreateDbContextAsync();

        SesionCaja sesion = await contexto.SesionesCaja.FirstOrDefaultAsync(s => s.FechaCierre == null)
            ?? throw new ReglaDeNegocioException("No hay una caja abierta para cerrar.");

        (decimal ingresos, decimal egresos) = await ReglasCaja.ObtenerTotalesAsync(contexto, sesion.Id);
        decimal saldoEsperado = sesion.MontoInicial + ingresos - egresos;

        sesion.FechaCierre = DateTime.UtcNow;
        sesion.EfectivoContado = efectivoContado;
        sesion.Diferencia = efectivoContado - saldoEsperado;

        await contexto.SaveChangesAsync();

        return new ResumenCaja(sesion, ingresos, egresos);
    }

    public async Task<PagedResult<MovimientoCaja>> ObtenerMovimientosAsync(int sesionId, int pagina, int tamanioPagina)
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

        IQueryable<MovimientoCaja> consulta = contexto.MovimientosCaja
            .AsNoTracking()
            .Where(m => m.SesionCajaId == sesionId);

        int total = await consulta.CountAsync();

        List<MovimientoCaja> items = await consulta
            .OrderByDescending(m => m.Fecha)
            .ThenByDescending(m => m.Id)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return new PagedResult<MovimientoCaja>
        {
            Items = items,
            TotalItems = total,
            Pagina = pagina,
            TamanioPagina = tamanioPagina
        };
    }

    public async Task<PagedResult<ResumenCaja>> ObtenerHistorialAsync(int pagina, int tamanioPagina)
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

        IQueryable<SesionCaja> consulta = contexto.SesionesCaja
            .AsNoTracking()
            .Where(s => s.FechaCierre != null);

        int total = await consulta.CountAsync();

        List<SesionCaja> sesiones = await consulta
            .OrderByDescending(s => s.FechaApertura)
            .ThenByDescending(s => s.Id)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        List<int> ids = sesiones.Select(s => s.Id).ToList();

        // Un solo GroupBy trae los totales de ingresos y egresos de todas las sesiones de la página.
        var totales = await contexto.MovimientosCaja
            .AsNoTracking()
            .Where(m => ids.Contains(m.SesionCajaId))
            .GroupBy(m => new { m.SesionCajaId, m.Tipo })
            .Select(g => new { g.Key.SesionCajaId, g.Key.Tipo, Total = g.Sum(m => m.Monto) })
            .ToListAsync();

        List<ResumenCaja> resumenes = sesiones
            .Select(s => new ResumenCaja(
                s,
                totales.Where(t => t.SesionCajaId == s.Id && t.Tipo == TipoMovimientoCaja.Ingreso).Sum(t => t.Total),
                totales.Where(t => t.SesionCajaId == s.Id && t.Tipo == TipoMovimientoCaja.Egreso).Sum(t => t.Total)))
            .ToList();

        return new PagedResult<ResumenCaja>
        {
            Items = resumenes,
            TotalItems = total,
            Pagina = pagina,
            TamanioPagina = tamanioPagina
        };
    }

    private async Task RegistrarMovimientoAsync(TipoMovimientoCaja tipo, string concepto, decimal monto)
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();

        SesionCaja? sesion = await contexto.SesionesCaja.FirstOrDefaultAsync(s => s.FechaCierre == null);

        decimal saldo = sesion is null
            ? 0m
            : await ReglasCaja.CalcularSaldoAsync(contexto, sesion);

        MovimientoCaja movimiento = ReglasCaja.CrearMovimiento(sesion, saldo, tipo, concepto, monto);
        contexto.MovimientosCaja.Add(movimiento);

        await contexto.SaveChangesAsync();
    }

    private static bool EsViolacionDeUnicidad(DbUpdateException ex)
    {
        return ex.InnerException is SqlException sql && (sql.Number == 2601 || sql.Number == 2627);
    }
}