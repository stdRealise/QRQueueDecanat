using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface ITicketService
{
    Task<TicketResponse?> GetTicketAsync(Guid ticketId,
        CancellationToken cancellationToken = default);
    Task<TicketResponse> CreateTicketAsync(Guid serviceId,
        CancellationToken cancellationToken = default);
    Task<TicketResponse> CancelTicketAsync(Guid ticketId,
        CancellationToken cancellationToken = default);
}