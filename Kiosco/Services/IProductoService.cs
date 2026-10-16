using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Operaciones de datos sobre los productos del kiosco.
/// </summary>
public interface IProductoService
{
    /// <summary>
    /// Obtiene una página de productos, con su categoría, ordenados por nombre.
    /// </summary>
    /// <param name="busqueda">Texto a buscar dentro del nombre. Si es nulo o vacío, no se filtra.</param>
    /// <param name="pagina">Número de página, empezando en 1.</param>
    /// <param name="tamanioPagina">Cantidad de productos por página.</param>
    /// <returns>La página pedida y el total de productos que cumplen la búsqueda.</returns>
    Task<PagedResult<Producto>> ObtenerPaginadoAsync(string? busqueda, int pagina, int tamanioPagina);
}