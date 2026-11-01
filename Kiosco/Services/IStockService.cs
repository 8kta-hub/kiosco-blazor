using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Operaciones sobre el stock de los productos y su historial de movimientos.
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Registra una entrada de mercadería (compra) y suma las unidades al stock.
    /// </summary>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="cantidad">Unidades que ingresan (mayor que cero).</param>
    /// <param name="observacion">Nota opcional.</param>
    /// <exception cref="ReglaDeNegocioException">Si la cantidad es inválida o el producto no existe.</exception>
    Task RegistrarEntradaAsync(int productoId, int cantidad, string? observacion);

    /// <summary>
    /// Registra una merma (producto perdido, vencido o roto) y resta las unidades al stock.
    /// </summary>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="cantidad">Unidades que se pierden (mayor que cero).</param>
    /// <param name="observacion">Nota opcional.</param>
    /// <exception cref="ReglaDeNegocioException">Si la cantidad es inválida, el producto no existe o el stock quedaría negativo.</exception>
    Task RegistrarMermaAsync(int productoId, int cantidad, string? observacion);

    /// <summary>
    /// Corrige el stock a la cantidad realmente contada, y registra la diferencia.
    /// </summary>
    /// <param name="productoId">Identificador del producto.</param>
    /// <param name="stockContado">Unidades que hay realmente (cero o más).</param>
    /// <param name="observacion">Nota opcional.</param>
    /// <exception cref="ReglaDeNegocioException">Si el valor es inválido, el producto no existe o no hay diferencia.</exception>
    Task RegistrarAjusteAsync(int productoId, int stockContado, string? observacion);

    /// <summary>
    /// Obtiene una página del historial de movimientos, del más nuevo al más viejo.
    /// </summary>
    /// <param name="productoId">Si tiene valor, solo los movimientos de ese producto.</param>
    /// <param name="tipo">Si tiene valor, solo los movimientos de ese tipo.</param>
    /// <param name="desdeUtc">Si tiene valor, movimientos desde ese momento (inclusive), en UTC.</param>
    /// <param name="hastaUtc">Si tiene valor, movimientos hasta ese momento (exclusivo), en UTC.</param>
    /// <param name="pagina">Número de página, empezando en 1.</param>
    /// <param name="tamanioPagina">Cantidad de movimientos por página.</param>
    /// <returns>La página pedida y el total de movimientos que cumplen los filtros.</returns>
    Task<PagedResult<MovimientoStock>> ObtenerHistorialAsync(
        int? productoId,
        TipoMovimientoStock? tipo,
        DateTime? desdeUtc,
        DateTime? hastaUtc,
        int pagina,
        int tamanioPagina);
}