using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Entrada o salida de efectivo dentro de una sesión de caja.
/// </summary>
public class MovimientoCaja
{
    public int Id { get; set; }

    public int SesionCajaId { get; set; }

    public SesionCaja? SesionCaja { get; set; }

    public TipoMovimientoCaja Tipo { get; set; }

    [Required(ErrorMessage = "El concepto es obligatorio.")]
    [StringLength(100, ErrorMessage = "El concepto no puede superar los 100 caracteres.")]
    public string Concepto { get; set; } = string.Empty;

    /// <summary>Importe siempre positivo: el <see cref="Tipo"/> indica si suma o resta.</summary>
    public decimal Monto { get; set; }

    /// <summary>Momento del movimiento, en UTC.</summary>
    public DateTime Fecha { get; set; }

    /// <summary>Venta que originó este ingreso. Es nulo en los movimientos manuales.</summary>
    public int? VentaId { get; set; }

    public Venta? Venta { get; set; }
}