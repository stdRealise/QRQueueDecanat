namespace QRQueueDecanat.DTOs;

public record StartOperatorSessionRequest(int CounterNumber);

public record OperatorSessionResponse(
    string OperatorName,
    int CounterNumber,
    DateTime StartedAt
);

public record OperatorWorkspaceResponse(
    OperatorTicketResponse? CurrentTicket,
    IReadOnlyList<OperatorTicketResponse> WaitingTickets
);

public record OperatorTicketResponse(
    Guid Id,
    string DisplayNumber,
    Guid ServiceId,
    string ServiceName,
    string StatusName,
    DateTime CreatedAt,
    DateTime? StartedAt,
    DateTime? EndedAt
);

public record UpdateOperatorServicesRequest(
    IReadOnlyCollection<Guid> ServiceIds
);

public record OperatorServiceResponse(
    Guid Id,
    string Name,
    string Prefix,
    string? Icon,
    bool IsSelected
);