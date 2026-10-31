using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Agrupa los productos del kiosco (por ejemplo: Bebidas, Golosinas).
/// </summary>
public class Categoria
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la categoría es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 50 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    public List<Producto> Productos { get; set; } = new();
}