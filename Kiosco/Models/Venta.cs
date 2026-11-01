namespace Kiosco.Models;

/// <summary>
/// Venta en efectivo realizada con una caja abierta.
/// </summary>
public class Venta
{
    public int Id { get; set; }

    /// <summary>Momento de la venta, en UTC.</summary>
    public DateTime Fecha { get; set; }

    /// <summary>Caja en la que se hizo la venta.</summary>
    public int SesionCajaId { get; set; }

    public SesionCaja? SesionCaja { get; set; }

    /// <summary>Suma de los subtotales de los ítems.</summary>
    public decimal Total { get; set; }

    /// <summary>Efectivo que entregó el cliente.</summary>
    public decimal MontoRecibido { get; set; }

    /// <summary>Efectivo que se devuelve al cliente: monto recibido menos total.</summary>
    public decimal Vuelto { get; set; }

    public List<VentaItem> Items { get; set; } = new();
}