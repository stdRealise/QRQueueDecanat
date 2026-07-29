using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Entities;
using QRQueueDecanat.Interfaces;
using QRQueueDecanat.Exceptions;

namespace QRQueueDecanat.Services;

public class TicketService : ITicketService
{
    private readonly ApplicationDbContext _context;
    private readonly TimeZoneInfo _queueTimeZone;
    private readonly IQueueNotifier _notifier;

    public TicketService(ApplicationDbContext context, 
        TimeZoneInfo queueTimeZone, IQueueNotifier notifier)
    {
        _context = context;
        _queueTimeZone = queueTimeZone;
        _notifier = notifier;
    }

    public async Task<TicketResponse?> GetTicketAsync(
        Guid ticketId, CancellationToken cancellationToken = default)
    {
        return await _context.Tickets
            .AsNoTracking()
            .Where(ticket => ticket.Id == ticketId)
            .Select(ticket => new TicketResponse(
                ticket.Id,
                ticket.DisplayNumber,
                ticket.ServiceId,
                ticket.Service.Name,
                ticket.Status.Name,
                ticket.CreatedAt,
                ticket.Session == null ? null : ticket.Session.Counter.Number
            )).SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<TicketResponse> CreateTicketAsync(
        Guid serviceId, CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);
        var service = await _context.Services
            .FromSqlInterpolated($"""
                SELECT *
                FROM services
                WHERE id = {serviceId}
                AND is_active = TRUE
                FOR UPDATE
            """).SingleOrDefaultAsync(cancellationToken);
        if (service is null)
        {
            throw new NotFoundException("Service not found.");
        }

        var waitingStatus = await _context.TicketStatuses
            .SingleAsync(status => status.Name == "waiting",
                cancellationToken);

        var utcNow = DateTime.UtcNow;
        var currentLocalTime = TimeZoneInfo.ConvertTimeFromUtc(
            utcNow, _queueTimeZone);
        var serviceDate = DateOnly.FromDateTime(currentLocalTime);

        var lastNumber = await _context.Tickets
            .Where(ticket => ticket.ServiceId == serviceId &&
                ticket.ServiceDate == serviceDate)
            .MaxAsync(ticket => (int?)ticket.Number, cancellationToken) ?? 0;
        var nextNumber = lastNumber + 1;
        var displayNumber = $"{service.Prefix}{nextNumber:D3}";

        var ticket = new Ticket
        {
            DisplayNumber = displayNumber,
            Number = nextNumber,
            ServiceId = service.Id,
            StatusId = waitingStatus.Id,
            ServiceDate = serviceDate,
            CreatedAt = utcNow
        };
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await _notifier.NotifyQueueChangedAsync(
            ticket.Id, cancellationToken);
        return new TicketResponse(
            ticket.Id,
            ticket.DisplayNumber,
            service.Id,
            service.Name,
            waitingStatus.Name,
            ticket.CreatedAt,
            null
        );
    }

    public async Task<TicketResponse> CancelTicketAsync(
        Guid ticketId, CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);
        var ticket = await _context.Tickets
            .FromSqlInterpolated($"""
                SELECT *
                FROM tickets
                WHERE id = {ticketId}
                FOR UPDATE
            """).SingleOrDefaultAsync(cancellationToken);
        if (ticket is null) {   
            throw new NotFoundException("Ticket not found.");
        }

        await _context.Entry(ticket)
            .Reference(item => item.Service)
            .LoadAsync(cancellationToken);
        await _context.Entry(ticket)
            .Reference(item => item.Status)
            .LoadAsync(cancellationToken);

        int? counterNumber = null;
        if (ticket.SessionId != null)
        {
            counterNumber = await _context.OperatorSessions
                .AsNoTracking()
                .Where(session => session.Id == ticket.SessionId.Value)
                .Select(session => (int?)session.Counter.Number)
                .SingleOrDefaultAsync(cancellationToken);
        }
        
        var canCancel = ticket.Status.Name == "waiting" ||
            ticket.Status.Name == "called";
        if (!canCancel)
        {
            throw new ConflictException("Ticket cannot be canceled.");
        }
        
        var cancelledStatus = await _context.TicketStatuses
                .SingleAsync(status => status.Name == "cancelled",
                    cancellationToken);
        ticket.StatusId = cancelledStatus.Id;
        ticket.Status = cancelledStatus;
        ticket.EndedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        await _notifier.NotifyQueueChangedAsync(ticket.Id, cancellationToken);
        return new TicketResponse(
            ticket.Id,
            ticket.DisplayNumber,
            ticket.ServiceId,
            ticket.Service.Name,
            cancelledStatus.Name,
            ticket.CreatedAt,
            counterNumber
        );
    }
}