using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Validaciones propias reutilizables para importes de dinero.
/// </summary>
public static class ValidacionesDinero
{
    /// <summary>
    /// Valida que el importe tenga como máximo 2 decimales.
    /// </summary>
    public static ValidationResult? ValidarDosDecimales(decimal monto, ValidationContext contexto)
    {
        if (decimal.Round(monto, 2) == monto)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            "El monto puede tener como máximo 2 decimales.",
            new[] { contexto.MemberName ?? "Monto" });
    }
}