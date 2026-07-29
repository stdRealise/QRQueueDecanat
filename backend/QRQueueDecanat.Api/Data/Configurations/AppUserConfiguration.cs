using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class AppUserConfiguration
    : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("users");
        builder.HasKey(user => user.Id);
        builder.Property(user => user.Username).HasMaxLength(100);
        builder.Property(user => user.PasswordHash).HasMaxLength(255);
        builder.Property(user => user.FullName).HasMaxLength(255);
        builder.Property(user => user.Email).HasMaxLength(255);

        builder.Property(user => user.IsActive).HasDefaultValue(true);
        builder.Property(user => user.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");
        
        builder.HasIndex(user => user.Username).IsUnique();

        builder.HasOne(user => user.Role)
            .WithMany(role => role.Users)
            .HasForeignKey(user => user.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}