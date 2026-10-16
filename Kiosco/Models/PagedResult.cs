namespace Kiosco.Models;

/// <summary>
/// Una página de resultados, con los datos necesarios para armar la paginación.
/// </summary>
/// <typeparam name="T">Tipo de elemento de la lista.</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();

    public int TotalItems { get; set; }

    public int Pagina { get; set; } = 1;

    public int TamanioPagina { get; set; } = 10;

    public int TotalPaginas => TamanioPagina <= 0
        ? 0
        : (int)Math.Ceiling(TotalItems / (double)TamanioPagina);
}