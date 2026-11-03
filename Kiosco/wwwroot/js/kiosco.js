// Módulo ES6 del Kiosco. Se carga bajo demanda desde C# con
// IJSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/kiosco.js").
// No define nada en el ámbito global.

const CLAVE_TEMA = 'kiosco-tema';

// ---------------------------------------------------------------------------
// Tema claro y oscuro
// ---------------------------------------------------------------------------

// Aplica el tema en la etiqueta <html>.
// data-tema lo usan nuestras variables CSS; data-bs-theme lo usa Bootstrap 5.3 o superior.
function aplicarTema(tema) {
    const raiz = document.documentElement;
    raiz.setAttribute('data-tema', tema);
    raiz.setAttribute('data-bs-theme', tema === 'oscuro' ? 'dark' : 'light');
}

// Lee el tema guardado. Si localStorage no está disponible, usa el claro.
function leerTemaGuardado() {
    try {
        return localStorage.getItem(CLAVE_TEMA) === 'oscuro' ? 'oscuro' : 'claro';
    } catch {
        return 'claro';
    }
}

// Lee el tema guardado, lo aplica y lo devuelve a C#.
export function iniciarTema() {
    const tema = leerTemaGuardado();
    aplicarTema(tema);
    return tema;
}

// Cambia entre claro y oscuro, guarda la elección y devuelve el tema nuevo.
export function alternarTema() {
    const actual = document.documentElement.getAttribute('data-tema');
    const nuevo = actual === 'oscuro' ? 'claro' : 'oscuro';
    aplicarTema(nuevo);
    try {
        localStorage.setItem(CLAVE_TEMA, nuevo);
    } catch {
        // Si no se puede guardar, el cambio vale solo para esta sesión.
    }
    return nuevo;
}

// ---------------------------------------------------------------------------
// Foco, portapapeles e impresión
// ---------------------------------------------------------------------------

// Pone el foco en el elemento con ese id. Devuelve false si no existe.
export function enfocarElemento(idElemento) {
    const elemento = document.getElementById(idElemento);
    if (!elemento) {
        return false;
    }

    elemento.focus();
    return true;
}

// Copia un texto al portapapeles. Devuelve false si el navegador lo rechaza.
export async function copiarAlPortapapeles(texto) {
    try {
        await navigator.clipboard.writeText(texto);
        return true;
    } catch {
        return false;
    }
}

// Abre el diálogo de impresión del navegador.
export function imprimirPagina() {
    window.print();
    return true;
}

// ---------------------------------------------------------------------------
// Avisos del navegador (API de Notificaciones)
// ---------------------------------------------------------------------------

// Devuelve 'granted', 'denied', 'default' o 'unsupported'.
export function obtenerPermisoNotificaciones() {
    return 'Notification' in window ? Notification.permission : 'unsupported';
}

// Pide permiso para mostrar avisos. Solo funciona como respuesta a un clic del usuario.
export async function pedirPermisoNotificaciones() {
    if (!('Notification' in window)) {
        return 'unsupported';
    }

    try {
        return await Notification.requestPermission();
    } catch {
        return Notification.permission;
    }
}

// Muestra un aviso si el usuario dio permiso. Devuelve false si no se pudo mostrar.
export function mostrarNotificacion(titulo, mensaje) {
    if (!('Notification' in window) || Notification.permission !== 'granted') {
        return false;
    }

    try {
        new Notification(titulo, { body: mensaje });
        return true;
    } catch {
        // Algunos navegadores móviles exigen un service worker para mostrar avisos.
        return false;
    }
}