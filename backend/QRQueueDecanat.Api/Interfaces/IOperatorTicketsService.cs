using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для работы оператора с талонами электронной очереди.
/// </summary>
public interface IOperatorTicketsService
{   
    /// <summary>
    /// Вызывает следующий ожидающий талон, соответствующий услугам оператора.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns> Данные вызванного талона в формате <see cref="OperatorTicketResponse"/> 
    /// или null, если подходящих талонов нет.</returns>
    Task<OperatorTicketResponse?> CallNextAsync(Guid operatorId,
        CancellationToken cancellationToken = default);
    /// <summary>
    /// Начинает обслуживание ранее вызванного талона.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="ticketId">Идентификатор талона.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Обновлённые данные талона в формате <see cref="OperatorTicketResponse"/>.</returns>
    Task<OperatorTicketResponse> StartTicketAsync(Guid operatorId, 
        Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Завершает обслуживание текущего талона.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="ticketId">Идентификатор талона.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Обновлённые данные завершённого талона в формате <see cref="OperatorTicketResponse"/>.
    /// </returns>
    Task<OperatorTicketResponse> CompleteTicketAsync(Guid operatorId, 
        Guid ticketId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Пропускает ранее вызванный талон.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="ticketId">Идентификатор талона.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>
    /// Обновлённые данные пропущенного талона в формате <see cref="OperatorTicketResponse"/>.
    /// </returns>
    Task<OperatorTicketResponse> SkipTicketAsync(Guid operatorId,
        Guid ticketId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Возвращает историю талонов, обработанных в текущей рабочей сессии оператора.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Список обработанных талонов в формате <see cref="OperatorTicketResponse"/>.
    /// </returns>
    Task<List<OperatorTicketResponse>> GetHistoryAsync(Guid operatorId,
        CancellationToken cancellationToken = default);
}