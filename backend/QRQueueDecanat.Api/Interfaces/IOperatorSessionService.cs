using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для управления рабочими сессиями операторов.
/// </summary>
public interface IOperatorSessionService
{
    /// <summary>
    /// Открывает рабочую сессию оператора в выбранном окне.
    /// </summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="request">Данные для открытия рабочей сессии.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные открытой сессии в формате <see cref="OperatorSessionResponse"/>.</returns>
    Task<OperatorSessionResponse> StartSessionAsync(
        Guid operatorId, StartOperatorSessionRequest request,
        CancellationToken cancellationToken = default);
    
    /// <summary>Закрывает текущую рабочую сессию оператора.</summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    Task CloseSessionAsync(Guid operatorId, 
        CancellationToken cancellationToken = default);
    
    /// <summary>Возвращает текущее рабочее пространство оператора.</summary>
    /// <param name="operatorId">Идентификатор оператора.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные рабочего пространства в формате <see cref="OperatorWorkspaceResponse"/>.</returns>
    Task<OperatorWorkspaceResponse> GetWorkspaceAsync(
        Guid operatorId, CancellationToken cancellationToken = default);
}