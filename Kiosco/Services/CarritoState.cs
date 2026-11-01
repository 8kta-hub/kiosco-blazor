using Kiosco.Models;

namespace Kiosco.Services;

/// <summary>
/// Carrito de la venta en curso. Se registra como Scoped: hay una instancia por usuario conectado.
/// Avisa mediante <see cref="OnChange"/> cada vez que cambia.
/// </summary>
public class CarritoState
{
    private readonly List<LineaCarrito> _lineas = new();

    /// <summary>Se dispara cada vez que el carrito cambia.</summary>
    public event Action? OnChange;

    public IReadOnlyList<LineaCarrito> Lineas => _lineas.AsReadOnly();

    public bool EstaVacio => _lineas.Count == 0;

    public decimal Total => _lineas.Sum(l => l.Subtotal);

    public int CantidadUnidades => _lineas.Sum(l => l.Cantidad);

    /// <summary>
    /// Agrega unidades de un producto al carrito.
    /// </summary>
    /// <param name="producto">Producto a agregar.</param>
    /// <param name="cantidad">Unidades a sumar (mayor que cero).</param>
    /// <returns><c>false</c> si la cantidad es inválida o el stock no alcanza para sumarla.</returns>
    public bool Agregar(Producto producto, int cantidad = 1)
    {
        if (cantidad < 1)
        {
            return false;
        }

        LineaCarrito? linea = _lineas.FirstOrDefault(l => l.ProductoId == producto.Id);
        int actual = linea?.Cantidad ?? 0;

        if (actual + cantidad > producto.StockActual)
        {
            return false;
        }

        if (linea is null)
        {
            linea = new LineaCarrito
            {
                ProductoId = producto.Id,
                Nombre = producto.Nombre
            };

            _lineas.Add(linea);
        }

        linea.PrecioUnitario = producto.PrecioVenta;
        linea.StockDisponible = producto.StockActual;
        linea.Cantidad = actual + cantidad;

        Notificar();
        return true;
    }

    /// <summary>
    /// Fija la cantidad de un producto del carrito. Con cantidad menor que 1, quita la línea.
    /// </summary>
    /// <returns><c>false</c> si el producto no está en el carrito o la cantidad supera el stock.</returns>
    public bool CambiarCantidad(int productoId, int cantidad)
    {
        LineaCarrito? linea = _lineas.FirstOrDefault(l => l.ProductoId == productoId);
        if (linea is null)
        {
            return false;
        }

        if (cantidad < 1)
        {
            Quitar(productoId);
            return true;
        }

        if (cantidad > linea.StockDisponible)
        {
            return false;
        }

        linea.Cantidad = cantidad;
        Notificar();
        return true;
    }

    /// <summary>
    /// Quita un producto del carrito.
    /// </summary>
    public void Quitar(int productoId)
    {
        if (_lineas.RemoveAll(l => l.ProductoId == productoId) > 0)
        {
            Notificar();
        }
    }

    /// <summary>
    /// Vacía el carrito.
    /// </summary>
    public void Vaciar()
    {
        if (_lineas.Count == 0)
        {
            return;
        }

        _lineas.Clear();
        Notificar();
    }

    /// <summary>
    /// Convierte el carrito en la lista de productos y cantidades que recibe el servicio de ventas.
    /// </summary>
    public IReadOnlyList<ItemVentaSolicitado> ComoItemsSolicitados()
    {
        return _lineas
            .Select(l => new ItemVentaSolicitado(l.ProductoId, l.Cantidad))
            .ToList();
    }

    private void Notificar()
    {
        OnChange?.Invoke();
    }
}