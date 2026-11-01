using System.Globalization;
using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <summary>
/// Reglas del dinero en efectivo compartidas por todos los servicios que mueven la caja.
/// </summary>
public static class ReglasCaja
{
    private const decimal MontoMaximo = 100_000_000m;

    private static readonly CultureInfo Cultura = new("es-AR");

    /// <summary>
    /// Calcula el total de ingresos y de egresos de una sesión.
    /// </summary>
    public static async Task<(decimal Ingresos, decimal Egresos)> ObtenerTotalesAsync(
        KioscoDbContext contexto,
        int sesionId)
    {
        decimal ingresos = await contexto.MovimientosCaja
            .Where(m => m.SesionCajaId == sesionId && m.Tipo == TipoMovimientoCaja.Ingreso)
            .SumAsync(m => m.Monto);

        decimal egresos = await contexto.MovimientosCaja
            .Where(m => m.SesionCajaId == sesionId && m.Tipo == TipoMovimientoCaja.Egreso)
            .SumAsync(m => m.Monto);

        return (ingresos, egresos);
    }

    /// <summary>
    /// Calcula el saldo de una sesión: monto inicial más ingresos menos egresos.
    /// </summary>
    public static async Task<decimal> CalcularSaldoAsync(KioscoDbContext contexto, SesionCaja sesion)
    {
        (decimal ingresos, decimal egresos) = await ObtenerTotalesAsync(contexto, sesion.Id);
        return sesion.MontoInicial + ingresos - egresos;
    }

    /// <summary>
    /// Valida un importe de dinero: no negativo, con a lo sumo 2 decimales y de tamaño razonable.
    /// </summary>
    /// <param name="monto">Importe a validar.</param>
    /// <param name="permitirCero">Si es <c>true</c>, el cero es válido (por ejemplo, una caja que abre sin efectivo).</param>
    /// <exception cref="ReglaDeNegocioException">Si el importe no es válido.</exception>
    public static void ValidarMonto(decimal monto, bool permitirCero)
    {
        if (monto < 0 || (!permitirCero && monto == 0))
        {
            throw new ReglaDeNegocioException("El monto debe ser mayor que cero.");
        }

        if (decimal.Round(monto, 2) != monto)
        {
            throw new ReglaDeNegocioException("El monto puede tener como máximo 2 decimales.");
        }

        if (monto > MontoMaximo)
        {
            throw new ReglaDeNegocioException("El monto es demasiado grande.");
        }
    }

    /// <summary>
    /// Crea el movimiento de dinero de una caja abierta, aplicando las reglas del negocio.
    /// No guarda nada: quien llama debe agregar el movimiento a su contexto y hacer un único SaveChanges.
    /// </summary>
    /// <param name="sesionAbierta">Sesión de caja abierta, o <c>null</c> si no hay ninguna.</param>
    /// <param name="saldoActual">Saldo actual de la sesión.</param>
    /// <param name="tipo">Ingreso o egreso.</param>
    /// <param name="concepto">Motivo del movimiento.</param>
    /// <param name="monto">Importe positivo.</param>
    /// <returns>El movimiento listo para guardar.</returns>
    /// <exception cref="ReglaDeNegocioException">
    /// Si no hay caja abierta, el importe o el concepto no son válidos o un egreso supera el saldo.
    /// </exception>
    public static MovimientoCaja CrearMovimiento(
        SesionCaja? sesionAbierta,
        decimal saldoActual,
        TipoMovimientoCaja tipo,
        string concepto,
        decimal monto)
    {
        if (sesionAbierta is null || !sesionAbierta.EstaAbierta)
        {
            throw new ReglaDeNegocioException(
                "No hay una caja abierta. Abrí la caja para registrar movimientos de dinero.");
        }

        ValidarMonto(monto, permitirCero: false);

        string texto = concepto?.Trim() ?? string.Empty;
        if (texto.Length == 0)
        {
            throw new ReglaDeNegocioException("El concepto es obligatorio.");
        }

        if (texto.Length > 100)
        {
            throw new ReglaDeNegocioException("El concepto no puede superar los 100 caracteres.");
        }

        if (tipo == TipoMovimientoCaja.Egreso && monto > saldoActual)
        {
            throw new ReglaDeNegocioException(
                $"El egreso ({monto.ToString("C", Cultura)}) supera el saldo de la caja ({saldoActual.ToString("C", Cultura)}).");
        }

        return new MovimientoCaja
        {
            SesionCaja = sesionAbierta,
            Tipo = tipo,
            Concepto = texto,
            Monto = monto,
            Fecha = DateTime.UtcNow
        };
    }
}