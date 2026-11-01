using System.Globalization;

namespace Kiosco.Services;

/// <summary>
/// Formatos de importes y fechas para mostrar en pantalla, con el estilo de Argentina.
/// </summary>
public static class Formato
{
    private static readonly CultureInfo Cultura = new("es-AR");

    /// <summary>
    /// Da formato de moneda a un importe (por ejemplo, $ 1.500,00).
    /// </summary>
    public static string Dinero(decimal monto)
    {
        return monto.ToString("C", Cultura);
    }

    /// <summary>
    /// Convierte una fecha guardada en UTC a la hora de Argentina y le da formato (por ejemplo, 01/11/2026 14:30).
    /// </summary>
    public static string FechaHora(DateTime fechaUtc)
    {
        return ZonaHoraria.AHoraLocal(fechaUtc).ToString("dd/MM/yyyy HH:mm", Cultura);
    }
}