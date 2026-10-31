using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Producto que se vende en el kiosco, con sus precios y su stock.
/// </summary>
public class Producto : IValidatableObject
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "El código de barras no puede superar los 50 caracteres.")]
    [CustomValidation(typeof(ValidacionesProducto), nameof(ValidacionesProducto.ValidarCodigoBarras))]
    public string? CodigoBarras { get; set; }

    [Range(0.01, 10000000, ErrorMessage = "El precio de costo debe ser mayor que cero.")]
    public decimal PrecioCosto { get; set; }

    [Range(0.01, 10000000, ErrorMessage = "El precio de venta debe ser mayor que cero.")]
    public decimal PrecioVenta { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock no puede ser negativo.")]
    public int StockActual { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo no puede ser negativo.")]
    public int StockMinimo { get; set; }

    public bool Activo { get; set; } = true;

    [Range(1, int.MaxValue, ErrorMessage = "Seleccioná una categoría.")]
    public int CategoriaId { get; set; }

    public Categoria? Categoria { get; set; }

    /// <summary>
    /// Validación entre campos: se ejecuta al enviar el formulario, cuando los atributos ya pasaron.
    /// </summary>
    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (PrecioVenta < PrecioCosto)
        {
            yield return new ValidationResult(
                "El precio de venta no puede ser menor que el costo.",
                new[] { nameof(PrecioVenta) });
        }
    }
}