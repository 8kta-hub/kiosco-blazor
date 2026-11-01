using System.ComponentModel.DataAnnotations;

namespace Kiosco.Models;

/// <summary>
/// Registro de un cambio en el stock de un producto.
/// </summary>
public class MovimientoStock
{
    public int Id { get; set; }

    public int ProductoId { get; set; }

    public Producto? Producto { get; set; }

    public TipoMovimientoStock Tipo { get; set; }

    public MotivoMovimientoStock Motivo { get; set; }

    /// <summary>
    /// Variación del stock con signo: positiva si suma unidades, negativa si resta.
    /// </summary>
    public int Cantidad { get; set; }

    /// <summary>
    /// Momento del movimiento, en UTC.
    /// </summary>
    public DateTime Fecha { get; set; }

    [StringLength(200, ErrorMessage = "La observación no puede superar los 200 caracteres.")]
    public string? Observacion { get; set; }
}