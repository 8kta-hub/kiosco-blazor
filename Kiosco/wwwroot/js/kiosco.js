// Módulo ES6 del Kiosco. Se carga bajo demanda desde los componentes
// con IJSRuntime.InvokeAsync<IJSObjectReference>("import", "./js/kiosco.js").

const CLAVE_TEMA = 'kiosco-tema';

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