namespace Kiosco.Models;

/// <summary>
/// Totales de una categoría: cuántos productos activos tiene y cuántas unidades hay en stock.
/// </summary>
/// <param name="Categoria">Nombre de la categoría.</param>
/// <param name="Productos">Cantidad de productos activos.</param>
/// <param name="Unidades">Suma del stock de esos productos.</param>
public record StockCategoria(string Categoria, int Productos, int Unidades);