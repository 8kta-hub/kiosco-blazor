namespace Kiosco.Services;

/// <summary>
/// Conversiones entre UTC y la hora de Argentina, para mostrar y filtrar fechas.
/// </summary>
public static class ZonaHoraria
{
    private static readonly TimeZoneInfo Argentina = Buscar();

    /// <summary>
    /// Convierte una fecha guardada en UTC a la hora de Argentina.
    /// </summary>
    public static DateTime AHoraLocal(DateTime fechaUtc)
    {
        DateTime utc = DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc);
        return TimeZoneInfo.ConvertTimeFromUtc(utc, Argentina);
    }

    /// <summary>
    /// Convierte una fecha y hora de Argentina a UTC.
    /// </summary>
    public static DateTime AUtc(DateTime fechaLocal)
    {
        DateTime sinZona = DateTime.SpecifyKind(fechaLocal, DateTimeKind.Unspecified);
        return TimeZoneInfo.ConvertTimeToUtc(sinZona, Argentina);
    }

    private static TimeZoneInfo Buscar()
    {
        foreach (string id in new[] { "America/Argentina/Buenos_Aires", "Argentina Standard Time" })
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById(id);
            }
            catch (TimeZoneNotFoundException)
            {
            }
            catch (InvalidTimeZoneException)
            {
            }
        }

        // Argentina no usa horario de verano: es UTC-3 todo el año.
        return TimeZoneInfo.CreateCustomTimeZone("ART", TimeSpan.FromHours(-3), "Argentina", "Argentina");
    }
}