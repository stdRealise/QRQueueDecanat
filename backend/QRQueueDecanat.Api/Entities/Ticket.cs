namespace QRQueueDecanat.Entities;

public class Ticket
{
    public Guid Id { get; set; }
    public string DisplayNumber { get; set; } = string.Empty;
    public int Number { get; set; }
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;

    public int StatusId { get; set; }
    public TicketStatus Status { get; set; } = null!;

    public Guid? SessionId { get; set; }
    public OperatorSession? Session { get; set; }
    
    public DateOnly ServiceDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
}