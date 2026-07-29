using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для получения данных для информационного табло очереди.
/// </summary>
public interface IPanelService
{
    /// <summary>
    /// Возвращает текущее состояние очереди для отображения на табло.
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Данные информационного табло в формате <see cref="PanelResponse"/>.</returns>
    Task<PanelResponse> GetPanelAsync(
        CancellationToken cancellationToken = default);
}