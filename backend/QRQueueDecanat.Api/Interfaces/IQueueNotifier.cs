using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IQueueNotifier
{
    Task NotifyQueueChangedAsync(Guid ticketId,
        CancellationToken cancellationToken = default);
}