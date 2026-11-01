namespace Kiosco.Models;

/// <summary>
/// Producto y cantidad que se quieren vender. El precio nunca viaja: lo toma el servicio de la base.
/// </summary>
/// <param name="ProductoId">Identificador del producto.</param>
/// <param name="Cantidad">Unidades a vender.</param>
public record ItemVentaSolicitado(int ProductoId, int Cantidad);