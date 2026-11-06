using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="ICategoriaService"/>
public class CategoriaService : ICategoriaService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;
    private readonly ILogger<CategoriaService> _logger;

    public CategoriaService(IDbContextFactory<KioscoDbContext> fabrica, ILogger<CategoriaService> logger)
    {
        _fabrica = fabrica;
        _logger = logger;
    }

    public Task<List<Categoria>> ObtenerTodasAsync()
    {
        return _logger.RegistrarLecturaAsync("Listar categorías", async () =>
        {
            await using var contexto = await _fabrica.CreateDbContextAsync();

            return await contexto.Categorias
                .AsNoTracking()
                .OrderBy(c => c.Nombre)
                .ToListAsync();
        });
    }
}