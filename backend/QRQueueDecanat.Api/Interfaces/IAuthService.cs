using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request,
        CancellationToken cancellationToken = default);
}