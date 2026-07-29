using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

/// <summary>
/// Сервис для аутентификации пользователей системы.
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Выполняет вход пользователя в систему.
    /// </summary>
    /// <param name="request">Данные для входа пользователя.</param>
    /// <param name="cancellationToken">Токен отмены операции.</param>
    /// <returns>Результат аутентификации в формате <see cref="LoginResponse"/>.</returns>
    Task<LoginResponse> LoginAsync(LoginRequest request,
        CancellationToken cancellationToken = default);
}