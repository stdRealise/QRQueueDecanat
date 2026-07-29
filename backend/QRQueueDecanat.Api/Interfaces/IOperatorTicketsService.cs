using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IOperatorTicketsService
{   
    Task<OperatorTicketResponse?> CallNextAsync(Guid operatorId,
        CancellationToken cancellationToken = default);
    Task<OperatorTicketResponse> StartTicketAsync(Guid operatorId, 
        Guid ticketId, CancellationToken cancellationToken = default);
    Task<OperatorTicketResponse> CompleteTicketAsync(Guid operatorId, 
        Guid ticketId, CancellationToken cancellationToken = default);
    Task<OperatorTicketResponse> SkipTicketAsync(Guid operatorId,
        Guid ticketId, CancellationToken cancellationToken = default);
    Task<List<OperatorTicketResponse>> GetHistoryAsync(Guid operatorId,
        CancellationToken cancellationToken = default);
}