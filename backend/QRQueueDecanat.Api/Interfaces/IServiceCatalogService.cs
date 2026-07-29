using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для работы с каталогом услуг.
/// </summary>
public interface IServiceCatalogService
{
    /// <summary>
    /// Возвращает список активных услуг, доступных для получения талона.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Коллекция активных услуг в формате<see cref="ServiceResponse"/>.</returns>
    Task<IEnumerable<ServiceResponse>> GetActiveServicesAsync(
        CancellationToken cancellationToken = default);
}