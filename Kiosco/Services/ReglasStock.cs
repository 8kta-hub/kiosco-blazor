using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Reglas del stock compartidas por todos los servicios que lo modifican.
/// </summary>
public static class ReglasStock
{
    /// <summary>
    /// Aplica una variación al stock del producto y devuelve el movimiento que la registra.
    /// No guarda nada: quien llama debe agregar el movimiento a su contexto y hacer un único SaveChanges.
    /// </summary>
    /// <param name="producto">Producto a modificar.</param>
    /// <param name="tipo">Entrada, salida o ajuste.</param>
    /// <param name="motivo">Razón del cambio.</param>
    /// <param name="variacion">Unidades con signo que se suman o se restan.</param>
    /// <param name="observacion">Nota opcional.</param>
    /// <returns>El movimiento listo para guardar.</returns>
    /// <exception cref="ReglaDeNegocioException">Si la variación es inválida o dejaría el stock negativo.</exception>
    public static MovimientoStock Aplicar(
        Producto producto,
        TipoMovimientoStock tipo,
        MotivoMovimientoStock motivo,
        int variacion,
        string? observacion)
    {
        if (variacion == 0)
        {
            throw new ReglaDeNegocioException("La cantidad no puede ser cero.");
        }

        if (tipo == TipoMovimientoStock.Entrada && variacion < 0)
        {
            throw new ReglaDeNegocioException("Una entrada debe sumar unidades.");
        }

        if (tipo == TipoMovimientoStock.Salida && variacion > 0)
        {
            throw new ReglaDeNegocioException("Una salida debe restar unidades.");
        }

        string? nota = string.IsNullOrWhiteSpace(observacion) ? null : observacion.Trim();
        if (nota is not null && nota.Length > 200)
        {
            throw new ReglaDeNegocioException("La observación no puede superar los 200 caracteres.");
        }

        int nuevoStock = producto.StockActual + variacion;
        if (nuevoStock < 0)
        {
            throw new ReglaDeNegocioException(
                $"Stock insuficiente de «{producto.Nombre}»: hay {producto.StockActual} y se quieren sacar {-variacion}.");
        }

        producto.StockActual = nuevoStock;

        return new MovimientoStock
        {
            Producto = producto,
            Tipo = tipo,
            Motivo = motivo,
            Cantidad = variacion,
            Fecha = DateTime.UtcNow,
            Observacion = nota
        };
    }
}