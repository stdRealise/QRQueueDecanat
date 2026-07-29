using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QRQueueDecanat.Entities;

namespace QRQueueDecanat.Data.Configurations;

public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.HasKey(ticket => ticket.Id);

        builder.Property(ticket => ticket.DisplayNumber)
            .HasMaxLength(20);

        builder.Property(ticket => ticket.CreatedAt)
            .HasDefaultValueSql("CURRENT_TIMESTAMP");

        builder.HasIndex(ticket => new
        {
            ticket.ServiceId,
            ticket.ServiceDate,
            ticket.Number
        }).IsUnique();

        builder.HasIndex(ticket => new
        {
            ticket.ServiceDate,
            ticket.DisplayNumber
        }).IsUnique();

        builder.HasIndex(ticket => new
        {
            ticket.ServiceDate,
            ticket.StatusId
        });

        builder.HasOne(ticket => ticket.Service)
            .WithMany(service => service.Tickets)
            .HasForeignKey(ticket => ticket.ServiceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ticket => ticket.Status)
            .WithMany(status => status.Tickets)
            .HasForeignKey(ticket => ticket.StatusId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ticket => ticket.Session)
            .WithMany(session => session.Tickets)
            .HasForeignKey(ticket => ticket.SessionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}