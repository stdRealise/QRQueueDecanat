namespace QRQueueDecanat.DTOs;

public record CreateTicketRequest(Guid ServiceId);

public record TicketResponse(
    Guid Id,
    string DisplayNumber,
    Guid ServiceId,
    string ServiceName,
    string StatusName,
    DateTime CreatedAt,
    int? CounterNumber = null
);