using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Estadísticas del kiosco calculadas con consultas a la base de datos.
/// </summary>
public interface IEstadisticaService
{
    /// <summary>
    /// Obtiene los cuatro datos de la franja de resumen: productos activos, stock bajo,
    /// saldo de la caja abierta y ventas del día (según el día de Argentina).
    /// </summary>
    /// <returns>El resumen del encabezado.</returns>
    Task<ResumenEncabezado> ObtenerResumenEncabezadoAsync();

    /// <summary>
    /// Obtiene los datos del panel de inicio: productos con stock bajo, más vendidos de los últimos 7 días
    /// y stock por categoría.
    /// </summary>
    /// <returns>El panel de inicio.</returns>
    Task<PanelKiosco> ObtenerPanelAsync();
}