namespace QRQueueDecanat.Entities;

public class OperatorStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<OperatorSession> Sessions { get; set; }
        = new List<OperatorSession>();
}