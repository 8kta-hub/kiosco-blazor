using Kiosco.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace Kiosco.Services;

/// <summary>
/// Comprueba que la aplicación pueda conectarse a la base de datos. Se expone en la ruta /health.
/// </summary>
public class BaseDeDatosHealthCheck : IHealthCheck
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;
    private readonly ILogger<BaseDeDatosHealthCheck> _logger;

    public BaseDeDatosHealthCheck(IDbContextFactory<KioscoDbContext> fabrica, ILogger<BaseDeDatosHealthCheck> logger)
    {
        _fabrica = fabrica;
        _logger = logger;
    }

    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var contexto = await _fabrica.CreateDbContextAsync(cancellationToken);

            bool conecta = await contexto.Database.CanConnectAsync(cancellationToken);

            return conecta
                ? HealthCheckResult.Healthy("La base de datos responde.")
                : HealthCheckResult.Unhealthy("No se pudo conectar con la base de datos.");
        }
        catch (Exception ex)
        {
            // El detalle técnico queda en el registro; la respuesta pública no lo muestra.
            _logger.LogError(ex, "La comprobación de salud de la base de datos falló.");
            return HealthCheckResult.Unhealthy("Falló la comprobación de la base de datos.");
        }
    }
}