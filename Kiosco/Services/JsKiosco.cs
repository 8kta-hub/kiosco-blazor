using Microsoft.JSInterop;

namespace Kiosco.Services;

/// <summary>
/// Envoltorio en C# del módulo JavaScript kiosco.js. Carga el módulo una sola vez, bajo demanda,
/// y lo libera al cerrarse el circuito. Si una llamada falla, devuelve un valor por defecto en vez de romper la pantalla.
/// </summary>
public class JsKiosco : IAsyncDisposable
{
    private readonly IJSRuntime _js;
    private IJSObjectReference? _modulo;

    public JsKiosco(IJSRuntime js)
    {
        _js = js;
    }

    /// <summary>Pone el foco en el elemento con ese id. Devuelve <c>false</c> si no existe.</summary>
    public Task<bool> EnfocarAsync(string idElemento)
    {
        return InvocarAsync("enfocarElemento", false, idElemento);
    }

    /// <summary>Copia un texto al portapapeles. Devuelve <c>false</c> si el navegador lo rechaza.</summary>
    public Task<bool> CopiarAsync(string texto)
    {
        return InvocarAsync("copiarAlPortapapeles", false, texto);
    }

    /// <summary>Abre el diálogo de impresión del navegador.</summary>
    public Task<bool> ImprimirAsync()
    {
        return InvocarAsync("imprimirPagina", false);
    }

    /// <summary>Devuelve el permiso de avisos: granted, denied, default o unsupported.</summary>
    public Task<string> PermisoNotificacionesAsync()
    {
        return InvocarAsync("obtenerPermisoNotificaciones", "unsupported");
    }

    /// <summary>Pide permiso para mostrar avisos. Debe llamarse como respuesta a un clic.</summary>
    public Task<string> PedirPermisoNotificacionesAsync()
    {
        return InvocarAsync("pedirPermisoNotificaciones", "unsupported");
    }

    /// <summary>Muestra un aviso del navegador si hay permiso. Devuelve <c>false</c> si no se pudo mostrar.</summary>
    public Task<bool> NotificarAsync(string titulo, string mensaje)
    {
        return InvocarAsync("mostrarNotificacion", false, titulo, mensaje);
    }

    public async ValueTask DisposeAsync()
    {
        if (_modulo is null)
        {
            return;
        }

        try
        {
            await _modulo.DisposeAsync();
        }
        catch (JSDisconnectedException)
        {
            // El circuito ya se cerró: no hay nada que liberar.
        }
    }

    private async Task<IJSObjectReference> ObtenerModuloAsync()
    {
        _modulo ??= await _js.InvokeAsync<IJSObjectReference>("import", "./js/kiosco.js");
        return _modulo;
    }

    private async Task<T> InvocarAsync<T>(string funcion, T valorPorDefecto, params object?[] argumentos)
    {
        try
        {
            IJSObjectReference modulo = await ObtenerModuloAsync();
            return await modulo.InvokeAsync<T>(funcion, argumentos);
        }
        catch (JSException)
        {
            return valorPorDefecto;
        }
        catch (JSDisconnectedException)
        {
            return valorPorDefecto;
        }
        catch (InvalidOperationException)
        {
            // Todavía no hay navegador (prerenderizado).
            return valorPorDefecto;
        }
    }
}