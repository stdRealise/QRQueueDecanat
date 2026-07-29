using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class RoleConfiguration
    : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.HasKey(role => role.Id);

        builder.Property(role => role.Name).HasMaxLength(100);

        builder.HasIndex(role => role.Name).IsUnique();

        builder.HasData(
            new Role
            {
                Id = 1,
                Name = "admin"
            },
            new Role
            {
                Id = 2,
                Name = "operator"
            }
        );
    }
}