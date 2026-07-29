using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class OperatorSessionConfiguration
    : IEntityTypeConfiguration<OperatorSession>
{
    public void Configure(
        EntityTypeBuilder<OperatorSession> builder
    )
    {
        builder.HasKey(session => session.Id);

        builder.Property(session => session.StartedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasOne(session => session.Operator)
            .WithMany(user => user.OperatorSessions)
            .HasForeignKey(session => session.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(session => session.Counter)
            .WithMany(counter => counter.Sessions)
            .HasForeignKey(session => session.CounterId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(session => session.Status)
            .WithMany(status => status.Sessions)
            .HasForeignKey(session => session.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(session => session.OperatorId)
            .IsUnique()
            .HasFilter("ended_at IS NULL");

        builder.HasIndex(session => session.CounterId)
            .IsUnique()
            .HasFilter("ended_at IS NULL");
    }
}