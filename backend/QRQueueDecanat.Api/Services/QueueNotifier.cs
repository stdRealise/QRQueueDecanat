using Microsoft.AspNetCore.SignalR;
using QRQueueDecanat.Hubs;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class QueueNotifier : IQueueNotifier
{
    private readonly IHubContext<QueueHub> _hubContext;

    public QueueNotifier(IHubContext<QueueHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task NotifyQueueChangedAsync(Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        var changedAt = DateTime.UtcNow;
        await Task.WhenAll(
            _hubContext.Clients
                .Group(QueueHub.TicketGroup(ticketId))
                .SendAsync("TicketChanged", ticketId, cancellationToken),
            _hubContext.Clients
                .Group(QueueHub.OperatorsGroup)
                .SendAsync("QueueChanged", changedAt, cancellationToken),
            _hubContext.Clients
                .Group(QueueHub.PanelGroup)
                .SendAsync("QueueChanged", changedAt, cancellationToken)
        );
    }
}