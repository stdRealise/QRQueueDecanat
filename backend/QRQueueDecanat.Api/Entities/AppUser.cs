namespace QRQueueDecanat.Entities;

public class AppUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public int RoleId { get; set; }
    
    public Role Role { get; set; } = null!;
    
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; }

    public ICollection<OperatorSession> OperatorSessions { get; set; }
        = new List<OperatorSession>();
    public ICollection<OperatorService> OperatorServices { get; set; }
        = new List<OperatorService>();
}