namespace QRQueueDecanat.Entities;

public class OperatorSession
{
    public Guid Id { get; set; }
    public Guid OperatorId { get; set; }
    public AppUser Operator { get; set; } = null!;

    public Guid CounterId { get; set; }
    public Counter Counter { get; set; } = null!;

    public int StatusId { get; set; }
    public OperatorStatus Status { get; set; } = null!;

    public DateTime StartedAt { get; set; }
    public DateTime? EndedAt { get; set; }
    public ICollection<Ticket> Tickets { get; set; }
        = new List<Ticket>();
}