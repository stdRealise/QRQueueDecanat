using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class ServiceConfiguration
    : IEntityTypeConfiguration<Service>
{
    public void Configure(EntityTypeBuilder<Service> builder)
    {
        builder.HasKey(service => service.Id);

        builder.Property(service => service.Name).HasMaxLength(255);
        builder.Property(service => service.Prefix).HasMaxLength(10);
        builder.Property(service => service.IconKey).HasMaxLength(100);

        builder.Property(service => service.IsActive).HasDefaultValue(true);

        builder.HasIndex(service => service.Name).IsUnique();
        builder.HasIndex(service => service.Prefix).IsUnique();
    }
}