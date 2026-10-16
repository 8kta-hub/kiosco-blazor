using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="IProductoService"/>
public class ProductoService : IProductoService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;

    public ProductoService(IDbContextFactory<KioscoDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<PagedResult<Producto>> ObtenerPaginadoAsync(string? busqueda, int pagina, int tamanioPagina)
    {
        if (pagina < 1)
        {
            pagina = 1;
        }

        if (tamanioPagina < 1)
        {
            tamanioPagina = 10;
        }

        await using var contexto = await _fabrica.CreateDbContextAsync();

        IQueryable<Producto> consulta = contexto.Productos
            .AsNoTracking()
            .Include(p => p.Categoria);

        if (!string.IsNullOrWhiteSpace(busqueda))
        {
            string texto = busqueda.Trim();
            consulta = consulta.Where(p => p.Nombre.Contains(texto));
        }

        int total = await consulta.CountAsync();

        List<Producto> items = await consulta
            .OrderBy(p => p.Nombre)
            .ThenBy(p => p.Id)
            .Skip((pagina - 1) * tamanioPagina)
            .Take(tamanioPagina)
            .ToListAsync();

        return new PagedResult<Producto>
        {
            Items = items,
            TotalItems = total,
            Pagina = pagina,
            TamanioPagina = tamanioPagina
        };
    }
}