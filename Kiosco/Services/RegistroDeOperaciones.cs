using Microsoft.Extensions.Logging;

namespace Kiosco.Services;

/// <summary>
/// Métodos de extensión de <see cref="ILogger"/> que ejecutan una operación y registran su resultado.
/// </summary>
public static class RegistroDeOperaciones
{
    /// <summary>
    /// Ejecuta una operación que modifica datos y la registra: Information si sale bien,
    /// Warning si una regla del negocio la rechaza y Error si falla algo inesperado. Siempre vuelve a lanzar el error.
    /// </summary>
    /// <typeparam name="T">Tipo del resultado de la operación.</typeparam>
    /// <param name="logger">Registro del servicio que ejecuta la operación.</param>
    /// <param name="operacion">Descripción de lo que se hace, para el mensaje.</param>
    /// <param name="accion">La operación.</param>
    /// <returns>El resultado de la operación.</returns>
    public static async Task<T> RegistrarOperacionAsync<T>(this ILogger logger, string operacion, Func<Task<T>> accion)
    {
        try
        {
            T resultado = await accion();
            logger.LogInformation("{Operacion}: completada.", operacion);
            return resultado;
        }
        catch (ReglaDeNegocioException ex)
        {
            logger.LogWarning("{Operacion}: rechazada por una regla del negocio. {Motivo}", operacion, ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Operacion}: falló.", operacion);
            throw;
        }
    }

    /// <summary>
    /// Igual que la versión con resultado, para operaciones que no devuelven nada.
    /// </summary>
    public static async Task RegistrarOperacionAsync(this ILogger logger, string operacion, Func<Task> accion)
    {
        await logger.RegistrarOperacionAsync<object?>(operacion, async () =>
        {
            await accion();
            return null;
        });
    }

    /// <summary>
    /// Ejecuta una consulta de lectura. Solo registra si falla, para no llenar el registro de ruido.
    /// </summary>
    /// <typeparam name="T">Tipo del resultado de la consulta.</typeparam>
    /// <param name="logger">Registro del servicio que ejecuta la consulta.</param>
    /// <param name="operacion">Descripción de lo que se consulta, para el mensaje.</param>
    /// <param name="accion">La consulta.</param>
    /// <returns>El resultado de la consulta.</returns>
    public static async Task<T> RegistrarLecturaAsync<T>(this ILogger logger, string operacion, Func<Task<T>> accion)
    {
        try
        {
            return await accion();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "{Operacion}: falló.", operacion);
            throw;
        }
    }
}