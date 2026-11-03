namespace Kiosco.Models;

/// <summary>
/// Un producto con las unidades vendidas y el dinero que generó en un período.
/// </summary>
/// <param name="Nombre">Nombre del producto.</param>
/// <param name="Unidades">Unidades vendidas.</param>
/// <param name="Total">Dinero recaudado por ese producto.</param>
public record ProductoVendido(string Nombre, int Unidades, decimal Total);