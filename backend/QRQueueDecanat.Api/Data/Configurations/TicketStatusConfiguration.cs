using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class TicketStatusConfiguration
    : IEntityTypeConfiguration<TicketStatus>
{
    public void Configure(
        EntityTypeBuilder<TicketStatus> builder
    )
    {
        builder.HasKey(status => status.Id);
        
        builder.Property(status => status.Name).HasMaxLength(100);

        builder.HasIndex(status => status.Name).IsUnique();

        builder.HasData(
            new
            {
                Id = 1,
                Name = "waiting"
            },
            new
            {
                Id = 2,
                Name = "called"
            },
            new
            {
                Id = 3,
                Name = "serving"
            },
            new
            {
                Id = 4,
                Name = "completed"
            },
            new
            {
                Id = 5,
                Name = "skipped"
            },
            new
            {
                Id = 6,
                Name = "cancelled"
            }
        );
    }
}