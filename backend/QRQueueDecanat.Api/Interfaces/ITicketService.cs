using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для управления созданием, получением и отменой талонов электронной очереди.
/// </summary>
public interface ITicketService
{
    /// <summary>
    /// Возвращает информацию о талоне по его идентификатору.
    /// </summary>
    /// <param name="ticketId">Идентификатор талона.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Данные талона <see cref="TicketResponse"/> или null, 
    /// если талон не найден.
    /// </returns>
    Task<TicketResponse?> GetTicketAsync(Guid ticketId,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Создает новый талон для выбранной услуги.
    /// </summary>
    /// <param name="serviceId">Идентификатор выбранной услуги.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные созданного талона в формате <see cref="TicketResponse"/>.</returns>
    Task<TicketResponse> CreateTicketAsync(Guid serviceId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Отменяет существующий талон.
    /// </summary>
    /// <param name="ticketId">Идентификатор отменяемого талона.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные отмененного талона в формате <see cref="TicketResponse"/>.</returns>
    Task<TicketResponse> CancelTicketAsync(Guid ticketId,
        CancellationToken cancellationToken = default);
}