using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace QRQueueDecanat.Hubs;

public class QueueHub : Hub
{
    public const string OperatorsGroup = "operators";
    public const string PanelGroup = "panel";

    public static string TicketGroup(Guid ticketId)
    {
        return $"ticket:{ticketId:N}";
    }

    public Task SubscribeToTicket(Guid ticketId)
    {
        return Groups.AddToGroupAsync(
            Context.ConnectionId, TicketGroup(ticketId));
    }

    [Authorize(Roles = "operator")]
    public Task SubscribeToOperators()
    {
        return Groups.AddToGroupAsync(
            Context.ConnectionId, OperatorsGroup);
    }

    public Task SubscribeToPanel()
    {
        return Groups.AddToGroupAsync(
            Context.ConnectionId, PanelGroup);
    }
}