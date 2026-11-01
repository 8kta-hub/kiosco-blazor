using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Operaciones sobre la caja de efectivo: apertura, movimientos de dinero, cierre e historial.
/// </summary>
public interface ICajaService
{
    /// <summary>
    /// Obtiene la caja abierta con sus totales, o <c>null</c> si no hay ninguna.
    /// </summary>
    /// <returns>El resumen de la caja abierta.</returns>
    Task<ResumenCaja?> ObtenerResumenAbiertaAsync();

    /// <summary>
    /// Abre la caja con un monto inicial de efectivo.
    /// </summary>
    /// <param name="montoInicial">Efectivo con el que se abre (cero o más, con a lo sumo 2 decimales).</param>
    /// <returns>La sesión de caja creada.</returns>
    /// <exception cref="ReglaDeNegocioException">Si el monto no es válido o ya hay una caja abierta.</exception>
    Task<SesionCaja> AbrirAsync(decimal montoInicial);

    /// <summary>
    /// Registra un ingreso manual de efectivo en la caja abierta.
    /// </summary>
    /// <param name="concepto">Motivo del ingreso.</param>
    /// <param name="monto">Importe mayor que cero.</param>
    /// <exception cref="ReglaDeNegocioException">Si no hay caja abierta o los datos no son válidos.</exception>
    Task RegistrarIngresoAsync(string concepto, decimal monto);

    /// <summary>
    /// Registra un egreso de efectivo en la caja abierta. No puede superar el saldo.
    /// </summary>
    /// <param name="concepto">Motivo del egreso.</param>
    /// <param name="monto">Importe mayor que cero.</param>
    /// <exception cref="ReglaDeNegocioException">Si no hay caja abierta, los datos no son válidos o el egreso supera el saldo.</exception>
    Task RegistrarEgresoAsync(string concepto, decimal monto);

    /// <summary>
    /// Cierra la caja abierta con el efectivo contado y registra la diferencia contra el saldo esperado.
    /// </summary>
    /// <param name="efectivoContado">Efectivo que hay realmente (cero o más, con a lo sumo 2 decimales).</param>
    /// <returns>El resumen de la sesión ya cerrada.</returns>
    /// <exception cref="ReglaDeNegocioException">Si no hay caja abierta o el monto no es válido.</exception>
    Task<ResumenCaja> CerrarAsync(decimal efectivoContado);

    /// <summary>
    /// Obtiene una página de movimientos de una sesión, del más nuevo al más viejo.
    /// </summary>
    /// <param name="sesionId">Identificador de la sesión de caja.</param>
    /// <param name="pagina">Número de página, empezando en 1.</param>
    /// <param name="tamanioPagina">Cantidad de movimientos por página.</param>
    /// <returns>La página pedida y el total de movimientos de la sesión.</returns>
    Task<PagedResult<MovimientoCaja>> ObtenerMovimientosAsync(int sesionId, int pagina, int tamanioPagina);

    /// <summary>
    /// Obtiene una página del historial de cajas cerradas, de la más nueva a la más vieja.
    /// </summary>
    /// <param name="pagina">Número de página, empezando en 1.</param>
    /// <param name="tamanioPagina">Cantidad de sesiones por página.</param>
    /// <returns>La página pedida, con los totales de cada sesión.</returns>
    Task<PagedResult<ResumenCaja>> ObtenerHistorialAsync(int pagina, int tamanioPagina);
}