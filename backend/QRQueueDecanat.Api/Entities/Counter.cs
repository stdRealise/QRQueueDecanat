namespace QRQueueDecanat.Entities;

public class Counter
{
    public Guid Id { get; set; }
    public int Number { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<OperatorSession> Sessions { get; set; }
        = new List<OperatorSession>();
}