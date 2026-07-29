using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class CounterConfiguration
    : IEntityTypeConfiguration<Counter>
{
    public void Configure(EntityTypeBuilder<Counter> builder)
    {
        builder.HasKey(counter => counter.Id);

        builder.Property(counter => counter.IsActive)
            .HasDefaultValue(true);

        builder.HasIndex(counter => counter.Number)
            .IsUnique();
    }
}