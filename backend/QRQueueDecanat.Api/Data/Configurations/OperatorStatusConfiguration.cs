using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Constants;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class OperatorStatusConfiguration
    : IEntityTypeConfiguration<OperatorStatus>
{
    public void Configure(EntityTypeBuilder<OperatorStatus> builder)
    {
        builder.HasKey(status => status.Id);
        builder.Property(status => status.Name).HasMaxLength(100);
        builder.HasIndex(status => status.Name).IsUnique();
        builder.HasData(
            new
            {
                Id = 1,
                Name = StatusNames.Active
            },
            new
            {
                Id = 2,
                Name = StatusNames.Paused
            }
        );
    }
}