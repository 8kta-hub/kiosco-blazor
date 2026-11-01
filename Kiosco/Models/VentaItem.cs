namespace Kiosco.Models;

/// <summary>
/// Línea de una venta: un producto, su cantidad y el precio al que se vendió.
/// </summary>
public class VentaItem
{
    public int Id { get; set; }

    public int VentaId { get; set; }

    public Venta? Venta { get; set; }

    public int ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public int Cantidad { get; set; }

    /// <summary>Precio de venta del producto en el momento de la venta.</summary>
    public decimal PrecioUnitario { get; set; }

    public decimal Subtotal => Cantidad * PrecioUnitario;
}