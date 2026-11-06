using Kiosco.Data;
using Kiosco.Models;
using Microsoft.EntityFrameworkCore;

namespace Kiosco.Services;

/// <inheritdoc cref="IVentaService"/>
public class VentaService : IVentaService
{
    private readonly IDbContextFactory<KioscoDbContext> _fabrica;
    private readonly ILogger<VentaService> _logger;

    public VentaService(IDbContextFactory<KioscoDbContext> fabrica, ILogger<VentaService> logger)
    {
        _fabrica = fabrica;
        _logger = logger;
    }

    public Task<Venta> ConfirmarVentaAsync(IReadOnlyList<ItemVentaSolicitado> items, decimal montoRecibido)
    {
        return _logger.RegistrarOperacionAsync($"Confirmar venta ({items?.Count ?? 0} productos)", async () =>
        {
            if (items is null || items.Count == 0)
            {
                throw new ReglaDeNegocioException("El carrito está vacío.");
            }

            ReglasCaja.ValidarMonto(montoRecibido, permitirCero: false);

            // Si el mismo producto aparece más de una vez, se suman sus cantidades.
            var pedidos = items
                .GroupBy(i => i.ProductoId)
                .Select(g => new { ProductoId = g.Key, Cantidad = g.Sum(i => i.Cantidad) })
                .ToList();

            if (pedidos.Any(p => p.Cantidad <= 0))
            {
                throw new ReglaDeNegocioException("Las cantidades deben ser mayores que cero.");
            }

            await using var contexto = await _fabrica.CreateDbContextAsync();

            SesionCaja sesion = await contexto.SesionesCaja.FirstOrDefaultAsync(s => s.FechaCierre == null)
                ?? throw new ReglaDeNegocioException("No hay una caja abierta. Abrí la caja para poder vender.");

            List<int> ids = pedidos.Select(p => p.ProductoId).ToList();
            Dictionary<int, Producto> productos = await contexto.Productos
                .Where(p => ids.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id);

            var venta = new Venta
            {
                Fecha = DateTime.UtcNow,
                SesionCaja = sesion
            };

            foreach (var pedido in pedidos)
            {
                if (!productos.TryGetValue(pedido.ProductoId, out Producto? producto))
                {
                    throw new ReglaDeNegocioException("Uno de los productos del carrito ya no existe.");
                }

                if (!producto.Activo)
                {
                    throw new ReglaDeNegocioException($"«{producto.Nombre}» está inactivo y no se puede vender.");
                }

                // Descuenta el stock y devuelve el movimiento que lo registra (lanza si el stock no alcanza).
                MovimientoStock movimientoStock = ReglasStock.Aplicar(
                    producto,
                    TipoMovimientoStock.Salida,
                    MotivoMovimientoStock.Venta,
                    -pedido.Cantidad,
                    "Venta");

                contexto.MovimientosStock.Add(movimientoStock);

                // El precio sale de la base, nunca de la pantalla.
                venta.Items.Add(new VentaItem
                {
                    Producto = producto,
                    Cantidad = pedido.Cantidad,
                    PrecioUnitario = producto.PrecioVenta
                });
            }

            venta.Total = venta.Items.Sum(i => i.Subtotal);

            if (montoRecibido < venta.Total)
            {
                throw new ReglaDeNegocioException(
                    $"El monto recibido ({Formato.Dinero(montoRecibido)}) no alcanza para cubrir el total ({Formato.Dinero(venta.Total)}).");
            }

            venta.MontoRecibido = montoRecibido;
            venta.Vuelto = montoRecibido - venta.Total;

            // El efecto neto en caja es el total: el vuelto sale del dinero que entregó el cliente.
            decimal saldo = await ReglasCaja.CalcularSaldoAsync(contexto, sesion);
            MovimientoCaja ingreso = ReglasCaja.CrearMovimiento(
                sesion,
                saldo,
                TipoMovimientoCaja.Ingreso,
                "Venta",
                venta.Total);
            ingreso.Venta = venta;

            contexto.MovimientosCaja.Add(ingreso);
            contexto.Ventas.Add(venta);

            // Un solo SaveChanges: venta, ítems, stock, movimientos de stock e ingreso en caja, todo o nada.
            await contexto.SaveChangesAsync();

            return venta;
        });
    }

    public Task<PagedResult<Venta>> ObtenerPaginadoAsync(int pagina, int tamanioPagina)
    {
        return _logger.RegistrarLecturaAsync("Listar ventas", async () =>
        {
            if (pagina < 1)
            {
                pagina = 1;
            }

            if (tamanioPagina < 1)
            {
                tamanioPagina = 10;
            }

            await using var contexto = await _fabrica.CreateDbContextAsync();

            IQueryable<Venta> consulta = contexto.Ventas.AsNoTracking();

            int total = await consulta.CountAsync();

            List<Venta> items = await consulta
                .Include(v => v.Items)
                    .ThenInclude(i => i.Producto)
                .OrderByDescending(v => v.Fecha)
                .ThenByDescending(v => v.Id)
                .Skip((pagina - 1) * tamanioPagina)
                .Take(tamanioPagina)
                .ToListAsync();

            return new PagedResult<Venta>
            {
                Items = items,
                TotalItems = total,
                Pagina = pagina,
                TamanioPagina = tamanioPagina
            };
        });
    }
}