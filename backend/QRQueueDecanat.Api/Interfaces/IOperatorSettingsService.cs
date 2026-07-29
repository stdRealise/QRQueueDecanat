using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для управления услугами, доступными оператору.
/// </summary>
public interface IOperatorSettingsService
{
    /// <summary>
    /// Возвращает список услуг, назначенных оператору.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список услуг оператора в формате <see cref="OperatorServiceResponse"/>.</returns>
    Task<List<OperatorServiceResponse>> GetOperatorServicesAsync(
        Guid operatorId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновляет список услуг, доступных оператору.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="request">Данные для обновления списка услуг.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task UpdateOperatorServicesAsync(Guid operatorId, 
        UpdateOperatorServicesRequest request,
        CancellationToken cancellationToken = default);
}