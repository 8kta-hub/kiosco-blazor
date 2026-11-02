namespace Kiosco.Models;

/// <summary>
/// Línea del carrito: un producto con su cantidad y el precio con el que se agregó.
/// </summary>
public class LineaCarrito
{
    public int ProductoId { get; init; }

    public string Nombre { get; init; } = string.Empty;

    public decimal PrecioUnitario { get; set; }

    public int Cantidad { get; set; }

    /// <summary>Stock que había al agregar el producto. Es solo una ayuda visual: el servicio vuelve a verificarlo.</summary>
    public int StockDisponible { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}