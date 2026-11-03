namespace Kiosco.Models;

/// <summary>
/// Datos que se muestran en la franja de resumen del encabezado.
/// </summary>
/// <param name="ProductosActivos">Cantidad de productos activos.</param>
/// <param name="ProductosStockBajo">Productos activos con stock igual o menor al mínimo.</param>
/// <param name="SaldoCaja">Saldo de la caja abierta, o <c>null</c> si no hay caja abierta.</param>
/// <param name="VentasHoy">Cantidad de ventas del día.</param>
/// <param name="TotalVentasHoy">Suma de las ventas del día.</param>
public record ResumenEncabezado(
    int ProductosActivos,
    int ProductosStockBajo,
    decimal? SaldoCaja,
    int VentasHoy,
    decimal TotalVentasHoy);