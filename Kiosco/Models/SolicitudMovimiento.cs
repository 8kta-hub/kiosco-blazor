using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Datos que carga el usuario en el formulario de movimiento de stock.
/// </summary>
public class SolicitudMovimiento : IValidatableObject
{
    [Range(1, int.MaxValue, ErrorMessage = "Seleccioná un producto.")]
    public int ProductoId { get; set; }

    public OperacionStock Operacion { get; set; } = OperacionStock.Entrada;

    /// <summary>
    /// Unidades que entran o salen. En un ajuste, es el stock realmente contado.
    /// </summary>
    [Range(0, 1000000, ErrorMessage = "La cantidad debe ser un número entre 0 y 1.000.000.")]
    public int Cantidad { get; set; }

    [StringLength(200, ErrorMessage = "La observación no puede superar los 200 caracteres.")]
    public string? Observacion { get; set; }

    /// <summary>
    /// Validación entre campos: entradas y mermas necesitan al menos una unidad; un ajuste puede ser cero.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Operacion != OperacionStock.Ajuste && Cantidad < 1)
        {
            yield return new ValidationResult(
                "La cantidad debe ser al menos 1.",
                new[] { nameof(Cantidad) });
        }
    }
}