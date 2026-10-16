using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Validaciones propias reutilizables para el modelo de productos.
/// </summary>
public static class ValidacionesProducto
{
    /// <summary>
    /// Valida que el código de barras, si se completó, tenga solo números y 8, 12, 13 o 14 dígitos.
    /// </summary>
    public static ValidationResult? ValidarCodigoBarras(string? codigo, ValidationContext contexto)
    {
        if (string.IsNullOrWhiteSpace(codigo))
        {
            return ValidationResult.Success;
        }

        string limpio = codigo.Trim();
        bool soloNumeros = limpio.All(char.IsAsciiDigit);
        bool largoValido = limpio.Length is 8 or 12 or 13 or 14;

        if (soloNumeros && largoValido)
        {
            return ValidationResult.Success;
        }

        return new ValidationResult(
            "El código de barras debe tener solo números y 8, 12, 13 o 14 dígitos.",
            new[] { contexto.MemberName ?? nameof(Producto.CodigoBarras) });
    }
}