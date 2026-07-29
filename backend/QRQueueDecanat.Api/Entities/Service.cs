namespace QRQueueDecanat.Entities;

public class Service
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Prefix { get; set; } = string.Empty;
    public int? Minutes { get; set; }
    public string? IconKey { get; set; }
    public bool IsActive { get; set; } = true;
    public ICollection<Ticket> Tickets { get; set; }
        = new List<Ticket>();
    public ICollection<OperatorService> OperatorServices { get; set; }
        = new List<OperatorService>();
}