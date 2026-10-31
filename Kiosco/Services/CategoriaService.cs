using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="ICategoriaService"/>
public class CategoriaService : ICategoriaService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;

    public CategoriaService(IDbContextFactory<KioscoDbContext> fabrica)
    {
        _fabrica = fabrica;
    }

    public async Task<List<Categoria>> ObtenerTodasAsync()
    {
        await using var contexto = await _fabrica.CreateDbContextAsync();

        return await contexto.Categorias
            .AsNoTracking()
            .OrderBy(c => c.Nombre)
            .ToListAsync();
    }
}