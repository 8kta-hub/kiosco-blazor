using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Operaciones sobre las ventas en efectivo.
/// </summary>
public interface IVentaService
{
    /// <summary>
    /// Confirma una venta en una sola transacción: crea la venta y sus ítems, descuenta el stock
    /// (con su movimiento) y registra el ingreso en la caja abierta. Si algo falla, no se guarda nada.
    /// </summary>
    /// <param name="items">Productos y cantidades a vender. Los productos repetidos se suman.</param>
    /// <param name="montoRecibido">Efectivo que entrega el cliente (debe cubrir el total).</param>
    /// <returns>La venta creada, con sus ítems y productos.</returns>
    /// <exception cref="ReglaDeNegocioException">
    /// Si el carrito está vacío, no hay caja abierta, un producto no existe o está inactivo,
    /// no alcanza el stock o el monto recibido no cubre el total.
    /// </exception>
    Task<Venta> ConfirmarVentaAsync(IReadOnlyList<ItemVentaSolicitado> items, decimal montoRecibido);

    /// <summary>
    /// Obtiene una página de ventas, de la más nueva a la más vieja, con sus ítems.
    /// </summary>
    /// <param name="pagina">Número de página, empezando en 1.</param>
    /// <param name="tamanioPagina">Cantidad de ventas por página.</param>
    /// <returns>La página pedida y el total de ventas.</returns>
    Task<PagedResult<Venta>> ObtenerPaginadoAsync(int pagina, int tamanioPagina);
}