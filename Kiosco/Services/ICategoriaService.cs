using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Operaciones de datos sobre las categorías de productos.
/// </summary>
public interface ICategoriaService
{
    /// <summary>
    /// Obtiene todas las categorías, ordenadas por nombre.
    /// </summary>
    /// <returns>Lista de categorías.</returns>
    Task<List<Categoria>> ObtenerTodasAsync();
}