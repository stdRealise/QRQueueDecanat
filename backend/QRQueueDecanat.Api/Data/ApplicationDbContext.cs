using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
    ) : base(options) {}

    public DbSet<Service> Services => Set<Service>();
    public DbSet<Ticket> Tickets => Set<Ticket>();
    public DbSet<TicketStatus> TicketStatuses => Set<TicketStatus>();

    public DbSet<Counter> Counters => Set<Counter>();

    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Role> Roles => Set<Role>();

    public DbSet<OperatorStatus> OperatorStatuses
        => Set<OperatorStatus>();
    public DbSet<OperatorSession> OperatorSessions
        => Set<OperatorSession>();
    public DbSet<OperatorService> OperatorServices
        => Set<OperatorService>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);
    }
}