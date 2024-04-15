using System.Diagnostics.CodeAnalysis;

namespace $safeprojectname$.Services.Audit;

/// <summary>
/// Servicio para encapsular un buffer concurrente con los log de auditoría, mediante este servicio se podra ir añadiendo a un buffer las auditorias de los
/// accesos a los endpoints y posteriormente podrán ser extraidos del buffer para un almacenamiento persistente. De este modo se optimiza el almacenamiento
/// persistente y no afectará las peticiones http de los clientes evitando tener que almacenar en cada petición la auditoría.
/// </summary>
/// <remarks>
/// Internamente el buffer debe ser único para toda la aplicación, no obstante es recomendable registrar este servicio en ID como singleton.
/// El buffer debe ser seguro en operaciones concurrentes
/// </remarks>
public interface IAuditBufferService
{
    /// <summary>
    /// Añadir una entrada de auditoria
    /// </summary>
    /// <param name="entry">entrada de auditoría</param>
    void Add(AuditDto entry);

    /// <summary>
    /// Función para obtener todas las entradas existentes en el buffer, esta operación vaciara el buffer.
    /// </summary>
    /// <returns>Enumeracion de auditorias obtenidas</returns>
    IEnumerable<AuditDto> TakeAll();

    /// <summary>
    /// Función para obtener la ultima entrada del buffer sin eliminarla del mismo.
    /// </summary>
    /// <param name="entry">Entrada obtenida</param>
    /// <returns>true si tiene exito, false en caso contrario</returns>
    bool TryPeek([MaybeNullWhen(false)] out AuditDto entry);

    /// <summary>
    /// Función para obtener la última entrada del buffer eliminándola del mismo.
    /// </summary>
    /// <param name="entry">Entrada obtenida</param>
    /// <returns>true si tiene exito, false en caso contrario</returns>
    bool TryTake([MaybeNullWhen(false)] out AuditDto entry);
}
