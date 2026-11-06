namespace Kiosco.Models;

/// <summary>
/// Datos del panel de la página de inicio.
/// </summary>
public class PanelKiosco
{
    /// <summary>Productos activos con stock igual o menor al mínimo, los más críticos primero.</summary>
    public List<Producto> StockBajo { get; init; } = new();

    /// <summary>Los productos más vendidos de los últimos 7 días.</summary>
    public List<ProductoVendido> MasVendidos { get; init; } = new();

    /// <summary>Productos y unidades en stock por categoría.</summary>
    public List<StockCategoria> StockPorCategoria { get; init; } = new();
}