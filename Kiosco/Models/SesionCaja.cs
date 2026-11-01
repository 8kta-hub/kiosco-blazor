namespace Kiosco.Models;

/// <summary>
/// Período en que la caja está abierta: va desde la apertura hasta el cierre con arqueo.
/// </summary>
public class SesionCaja
{
    public int Id { get; set; }

    /// <summary>Momento de apertura, en UTC.</summary>
    public DateTime FechaApertura { get; set; }

    /// <summary>Efectivo con el que se abre la caja.</summary>
    public decimal MontoInicial { get; set; }

    /// <summary>Momento de cierre, en UTC. Es nulo mientras la caja está abierta.</summary>
    public DateTime? FechaCierre { get; set; }

    /// <summary>Efectivo contado al cerrar. Es nulo mientras la caja está abierta.</summary>
    public decimal? EfectivoContado { get; set; }

    /// <summary>Efectivo contado menos saldo esperado: positiva si sobra, negativa si falta.</summary>
    public decimal? Diferencia { get; set; }

    public bool EstaAbierta => FechaCierre is null;

    public List<MovimientoCaja> Movimientos { get; set; } = new();
}