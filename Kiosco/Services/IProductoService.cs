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

    /// <summary>
    /// Busca un producto por su identificador.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <returns>El producto, o <c>null</c> si no existe.</returns>
    Task<Producto?> ObtenerPorIdAsync(int id);

    /// <summary>
    /// Crea un producto nuevo.
    /// </summary>
    /// <param name="producto">Datos del producto (incluye el stock inicial).</param>
    /// <returns>El producto creado, con su identificador asignado.</returns>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Si falla el guardado en la base.</exception>
    Task<Producto> CrearAsync(Producto producto);

    /// <summary>
    /// Actualiza los datos editables de un producto. No modifica el stock actual.
    /// </summary>
    /// <param name="producto">Producto con los valores nuevos.</param>
    /// <exception cref="InvalidOperationException">Si el producto ya no existe.</exception>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Si falla el guardado en la base.</exception>
    Task ActualizarAsync(Producto producto);

    /// <summary>
    /// Elimina un producto. Si no existe, no hace nada.
    /// </summary>
    /// <param name="id">Identificador del producto.</param>
    /// <exception cref="Microsoft.EntityFrameworkCore.DbUpdateException">Si falla el guardado en la base.</exception>
    Task EliminarAsync(int id);
}