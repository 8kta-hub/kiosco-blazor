using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Un importe cargado por el usuario: el monto inicial al abrir la caja o el efectivo contado al cerrarla.
/// </summary>
public class SolicitudMonto
{
    [Range(0, 100000000, ErrorMessage = "El monto debe estar entre 0 y 100.000.000.")]
    [CustomValidation(typeof(ValidacionesDinero), nameof(ValidacionesDinero.ValidarDosDecimales))]
    public decimal Monto { get; set; }
}