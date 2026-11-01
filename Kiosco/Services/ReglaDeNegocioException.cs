namespace Kiosco.Services;

/// <summary>
/// Se lanza cuando una operación viola una regla del negocio.
/// Su mensaje está pensado para mostrarse al usuario.
/// </summary>
public class ReglaDeNegocioException : Exception
{
    public ReglaDeNegocioException(string mensaje)
        : base(mensaje)
    {
    }
}