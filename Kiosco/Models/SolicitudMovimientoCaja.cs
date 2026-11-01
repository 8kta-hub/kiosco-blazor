using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Datos que carga el usuario en el formulario de ingreso o egreso de efectivo.
/// </summary>
public class SolicitudMovimientoCaja
{
    public TipoMovimientoCaja Tipo { get; set; } = TipoMovimientoCaja.Ingreso;

    [Required(ErrorMessage = "El concepto es obligatorio.")]
    [StringLength(100, ErrorMessage = "El concepto no puede superar los 100 caracteres.")]
    public string Concepto { get; set; } = string.Empty;

    [Range(0.01, 100000000, ErrorMessage = "El monto debe ser mayor que cero.")]
    [CustomValidation(typeof(ValidacionesDinero), nameof(ValidacionesDinero.ValidarDosDecimales))]
    public decimal Monto { get; set; }
}