# Historias de usuario - Kiosco

Formato: Como [usuario], quiero [acción] para [beneficio].
Estimación en puntos (Fibonacci): 1, 2, 3, 5, 8, 13.

## Definition of Done (aplica a todas)

- La funcionalidad está implementada y probada manualmente.
- El código está en una rama `feature/*` con commits en formato Conventional Commits.
- Hay un Pull Request a `develop`, revisado por mí con al menos un comentario, y mergeado.
- Sin errores en la consola del navegador.
- La pantalla es usable a 375 px de ancho.
- No hay credenciales ni secretos en el código.
- README o CHANGELOG actualizados si corresponde.

---

## HU-01: Usar la aplicación cómodamente en el celular y de noche

**Como** kiosquero, **quiero** una interfaz clara y con modo oscuro **para** usarla desde el celular detrás del mostrador sin cansar la vista.

**Criterios de aceptación**
- El diseño es usable a 375 px de ancho.
- Hay un botón visible para alternar entre modo claro y oscuro.
- La elección de modo se mantiene al recargar la página.
- El menú de navegación está presente en todas las pantallas.

**Estimación:** 5

## HU-02: Gestionar el catálogo de productos

**Como** kiosquero, **quiero** crear, ver, editar y eliminar productos con su categoría **para** tener el catálogo actualizado.

**Criterios de aceptación**
- La lista de productos tiene paginación y búsqueda por nombre.
- El formulario valida nombre obligatorio, precio de venta mayor que cero y precio de venta no menor que el costo.
- Eliminar pide confirmación en un modal.
- Si falla la base de datos, se ve un mensaje de error en pantalla (no una pantalla de error).

**Estimación:** 8

## HU-03: Controlar el stock

**Como** kiosquero, **quiero** registrar entradas, mermas y ajustes de stock **para** saber cuánta mercadería tengo y cuándo reponer.

**Criterios de aceptación**
- El stock nunca puede quedar negativo.
- Cada cambio de stock deja un movimiento en el historial (tipo, motivo, cantidad, fecha).
- Los productos por debajo del stock mínimo se muestran con una alerta visual.
- El historial es paginado y se puede filtrar.

**Estimación:** 5

## HU-04: Controlar el efectivo de la caja

**Como** kiosquero, **quiero** abrir la caja, registrar ingresos y egresos y cerrarla con arqueo **para** saber si el dinero cuadra.

**Criterios de aceptación**
- Solo puede haber una caja abierta a la vez.
- No se puede mover dinero sin caja abierta.
- Un egreso no puede superar el saldo actual de la caja.
- Al cerrar se muestra el saldo esperado, el efectivo contado y la diferencia.

**Estimación:** 5

## HU-05: Registrar una venta en efectivo

**Como** kiosquero, **quiero** registrar una venta con monto recibido y vuelto **para** cobrar rápido y que stock y caja se actualicen solos.

**Criterios de aceptación**
- No se puede vender sin caja abierta ni sin stock suficiente.
- El sistema calcula el vuelto a partir del monto recibido.
- Al confirmar, se descuenta el stock y se registra el ingreso en caja de una sola vez; si algo falla, no se guarda nada.

**Estimación:** 8