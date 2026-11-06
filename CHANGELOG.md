# Changelog

Todos los cambios importantes de este proyecto se documentan en este archivo.

El formato se basa en [Keep a Changelog](https://keepachangelog.com/es-ES/1.1.0/) y el proyecto sigue el [Versionado Semántico](https://semver.org/lang/es/).

## [Unreleased]

## [1.0.0] - 2026-11-04

### Added
- Configuración por ambiente: `appsettings.json`, `appsettings.Development.json` y `appsettings.Production.json`.
- Registro de operaciones con `ILogger<T>` en todos los servicios, con niveles Information, Warning y Error.
- Endpoint `/health` con la comprobación de la base de datos.
- README profesional con capturas, diagramas Mermaid de arquitectura y modelo de datos, e instrucciones de instalación.
- Badge de estado del build en el README.

### Changed
- Los servicios registran sus operaciones y errores mediante un ayudante común (`RegistroDeOperaciones`).

## [0.5.0] - 2026-11-03

### Added
- Resumen en el encabezado (productos activos, stock bajo, saldo de caja y ventas de hoy), con actualización automática cada 15 segundos.
- Panel de inicio con stock bajo, productos más vendidos de los últimos 7 días y stock por categoría, calculados con consultas `GroupBy`.
- JS Interop: foco automático en el buscador, copiar el resumen de cierre de caja, imprimir el arqueo y el comprobante, y avisos del navegador por stock bajo.
- Estilos de impresión.

### Changed
- El carrito se reacomoda para pantallas de 375 px.

## [0.4.0] - 2026-11-02

### Added
- Caja en efectivo: apertura, ingresos y egresos con concepto, saldo en vivo, cierre con arqueo e historial.
- Regla de una sola caja abierta, garantizada también por un índice único filtrado en la base.
- Ventas en efectivo con carrito (`CarritoState`), monto recibido y vuelto; la venta, el stock y el ingreso en caja se guardan en una sola transacción.
- Historial de ventas.
- Migraciones `AgregarCaja` y `AgregarVentas`.

### Fixed
- Error de arranque en producción cuando la base no tenía los productos del seed: se restauraron los datos y se documentó el procedimiento.

## [0.3.0] - 2026-11-01

### Added
- Control de stock: movimientos de entrada, merma y ajuste, con historial paginado y filtrado.
- Alerta visual de stock bajo.
- La regla de que el stock nunca queda negativo y el registro de cada cambio en la misma transacción.
- Migración `AgregarMovimientosStock`.

### Changed
- Un producto con historial de stock ya no se puede eliminar: se desactiva.

## [0.2.0] - 2026-10-30

### Added
- Pipeline de CI/CD con GitHub Actions: restaurar, compilar, publicar y desplegar con Web Deploy.
- Despliegue de la aplicación en MonsterASP.NET con base de datos SQL Server.

## [0.1.0] - 2026-10-16

### Added
- Proyecto Blazor Server en .NET 10 con layout semántico, variables CSS, CSS Isolation y Bootstrap 5.
- Modo oscuro con persistencia en `localStorage` mediante JS Interop.
- Modelo de datos con EF Core: categorías y productos, `IDbContextFactory`, migración inicial y datos de ejemplo.
- CRUD de productos con búsqueda, paginación y validación avanzada, y modal de confirmación para eliminar.
- Componentes reutilizables: `AlertError`, `Paginacion`, `BadgeStock` y `ConfirmDialog`.
- Repositorio con Git Flow, historias de usuario y tablero Kanban.