namespace QRQueueDecanat.Entities;

public class OperatorService
{
    public Guid OperatorId { get; set; }
    public AppUser Operator { get; set; } = null!;
    public Guid ServiceId { get; set; }
    public Service Service { get; set; } = null!;
}