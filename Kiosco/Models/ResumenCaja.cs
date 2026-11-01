namespace Kiosco.Models;

/// <summary>
/// Una sesión de caja junto con sus totales de ingresos y egresos.
/// </summary>
public class ResumenCaja
{
    public ResumenCaja(SesionCaja sesion, decimal totalIngresos, decimal totalEgresos)
    {
        Sesion = sesion;
        TotalIngresos = totalIngresos;
        TotalEgresos = totalEgresos;
    }

    public SesionCaja Sesion { get; }

    public decimal TotalIngresos { get; }

    public decimal TotalEgresos { get; }

    /// <summary>Monto inicial más ingresos menos egresos.</summary>
    public decimal SaldoEsperado => Sesion.MontoInicial + TotalIngresos - TotalEgresos;
}