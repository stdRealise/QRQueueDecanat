using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для отправки уведомлений об изменениях в электронной очереди.
/// </summary>
public interface IQueueNotifier
{
    /// <summary>
    /// Уведомляет студента, оператора и информационное табло об изменении состояния талона и очереди.
    /// </summary>
    /// <param name="ticketId">Идентификатор изменённого талона.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task NotifyQueueChangedAsync(Guid ticketId,
        CancellationToken cancellationToken = default);
}