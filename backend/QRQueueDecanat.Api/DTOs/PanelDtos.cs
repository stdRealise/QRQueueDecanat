namespace QRQueueDecanat.DTOs;

public record PanelTicketResponse(
    Guid Id,
    string DisplayNumber,
    int CounterNumber
);

public record PanelResponse(
    int WorkingCounters,
    int WaitingCount,
    int ServingCount,
    int AverageWaitingMinutes,
    IReadOnlyList<PanelTicketResponse> Calls,
    IReadOnlyList<string> WaitingNumbers
);