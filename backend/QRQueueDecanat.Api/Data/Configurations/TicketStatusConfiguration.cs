using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Constants;
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
                Name = StatusNames.Waiting
            },
            new
            {
                Id = 2,
                Name = StatusNames.Called
            },
            new
            {
                Id = 3,
                Name = StatusNames.Serving
            },
            new
            {
                Id = 4,
                Name = StatusNames.Completed
            },
            new
            {
                Id = 5,
                Name = StatusNames.Skipped
            },
            new
            {
                Id = 6,
                Name = StatusNames.Cancelled
            }
        );
    }
}