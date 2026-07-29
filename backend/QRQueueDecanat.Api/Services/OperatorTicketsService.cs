using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Constants;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Entities;
using QRQueueDecanat.Exceptions;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class OperatorTicketsService : IOperatorTicketsService
{
    private readonly ApplicationDbContext _context;
    private readonly TimeZoneInfo _queueTimeZone;
    private readonly IQueueNotifier _notifier;

    public OperatorTicketsService(ApplicationDbContext context, 
        TimeZoneInfo queueTimeZone, IQueueNotifier notifier)
    {
        _context = context;
        _queueTimeZone = queueTimeZone;
        _notifier = notifier;
    }

    public async Task<OperatorTicketResponse?> CallNextAsync(
        Guid operatorId, CancellationToken cancellationToken = default)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);
        var session = await _context.OperatorSessions
            .FromSqlInterpolated($"""
                SELECT *
                FROM operator_sessions
                WHERE operator_id = {operatorId}
                AND ended_at IS NULL
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Session not found.");

        var calledStatus = await _context.TicketStatuses
            .SingleAsync(status => status.Name == StatusNames.Called,
                cancellationToken);
        var servingStatus = await _context.TicketStatuses
            .SingleAsync(status => status.Name == StatusNames.Serving,
                cancellationToken);
        
        var hasCurrentTicket = await _context.Tickets
            .AnyAsync(ticket =>
                ticket.SessionId == session.Id &&
                (ticket.StatusId == calledStatus.Id ||
                    ticket.StatusId == servingStatus.Id),
                cancellationToken);
        if (hasCurrentTicket)
        {
            throw new ConflictException(
                "Finish working with the current ticket first.");
        }

        var waitingStatus = await _context.TicketStatuses
            .SingleAsync(status => status.Name == StatusNames.Waiting,
                cancellationToken);
        var queueDate = GetQueueDate();
        var ticket = await _context.Tickets.FromSqlInterpolated($"""
            SELECT tickets.*
            FROM tickets
            INNER JOIN operator_services
                ON operator_services.service_id = tickets.service_id
            WHERE operator_services.operator_id = {session.OperatorId}
                AND tickets.status_id = {waitingStatus.Id}
                AND tickets.service_date = {queueDate}
            ORDER BY tickets.created_at
            FOR UPDATE OF tickets SKIP LOCKED
            LIMIT 1
            """).SingleOrDefaultAsync(cancellationToken);
        if (ticket is null)
        {
            await transaction.CommitAsync(cancellationToken);
            return null;
        }

        await _context.Entry(ticket)
            .Reference(item => item.Service)
            .LoadAsync(cancellationToken);
        ticket.StatusId = calledStatus.Id;
        ticket.Status = calledStatus;
        ticket.SessionId = session.Id;
        ticket.CalledAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await _notifier.NotifyQueueChangedAsync(
            ticket.Id, cancellationToken);
        return MapTicket(ticket);
    }

    public Task<OperatorTicketResponse> StartTicketAsync(
        Guid operatorId, Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        return ChangeTicketStatusAsync(
            operatorId,
            ticketId,
            StatusNames.Called,
            StatusNames.Serving,
            (ticket, utcNow) =>
            {
                ticket.StartedAt = utcNow;
            },
            cancellationToken
        );
    }

    public Task<OperatorTicketResponse> CompleteTicketAsync(
        Guid operatorId, Guid ticketId,
        CancellationToken cancellationToken = default)
    {
        return ChangeTicketStatusAsync(
            operatorId,
            ticketId,
            StatusNames.Serving,
            StatusNames.Completed,
            (ticket, utcNow) =>
            {
                ticket.EndedAt = utcNow;
            },
            cancellationToken
        );
    }

    public Task<OperatorTicketResponse> SkipTicketAsync(Guid operatorId,
        Guid ticketId, CancellationToken cancellationToken = default)
    {
        return ChangeTicketStatusAsync(
            operatorId,
            ticketId,
            StatusNames.Called,
            StatusNames.Skipped,
            (ticket, utcNow) =>
            {
                ticket.EndedAt = utcNow;
            },
            cancellationToken
        );
    }

    public async Task<List<OperatorTicketResponse>> GetHistoryAsync(
        Guid operatorId, CancellationToken cancellationToken = default)
    {
        var sessionId = await GetActiveSessionIdAsync(
            operatorId, cancellationToken);
        return await _context.Tickets
            .AsNoTracking()
            .Where(ticket =>
                ticket.SessionId == sessionId &&
                (
                    ticket.Status.Name == StatusNames.Completed ||
                    ticket.Status.Name == StatusNames.Skipped ||
                    ticket.Status.Name == StatusNames.Cancelled
                ))
            .OrderByDescending(ticket => ticket.EndedAt)
            .Select(ticket => new OperatorTicketResponse(
                ticket.Id,
                ticket.DisplayNumber,
                ticket.ServiceId,
                ticket.Service.Name,
                ticket.Status.Name,
                ticket.CreatedAt,
                ticket.StartedAt,
                ticket.EndedAt
            )).ToListAsync(cancellationToken);
    }

    private async Task<OperatorTicketResponse> ChangeTicketStatusAsync(
        Guid operatorId, Guid ticketId, string requiredStatusName,
        string targetStatusName, Action<Ticket, DateTime> changeTime,
        CancellationToken cancellationToken)
    {
        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);
        var session = await _context.OperatorSessions
            .FromSqlInterpolated($"""
                SELECT *
                FROM operator_sessions
                WHERE operator_id = {operatorId}
                AND ended_at IS NULL
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Session not found.");
        var ticket = await _context.Tickets
            .FromSqlInterpolated($"""
                SELECT *
                FROM tickets
                WHERE id = {ticketId}
                AND session_id = {session.Id}
                FOR UPDATE
                """)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Ticket not found.");

        await _context.Entry(ticket)
            .Reference(item => item.Service)
            .LoadAsync(cancellationToken);
        await _context.Entry(ticket)
            .Reference(item => item.Status)
            .LoadAsync(cancellationToken);

        if (ticket.Status.Name != requiredStatusName)
        {
            throw new ConflictException(
                "Action is not available for the current ticket status.");
        }
        var targetStatus = await _context.TicketStatuses
            .SingleAsync(status => status.Name == targetStatusName,
                cancellationToken);

        ticket.StatusId = targetStatus.Id;
        ticket.Status = targetStatus;
        var utcNow = DateTime.UtcNow;
        changeTime(ticket, utcNow);

        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
        await _notifier.NotifyQueueChangedAsync(
            ticket.Id, cancellationToken);
        return MapTicket(ticket);
    }

    private OperatorTicketResponse MapTicket(Ticket ticket)
    {
        return new OperatorTicketResponse(
            ticket.Id,
            ticket.DisplayNumber,
            ticket.ServiceId,
            ticket.Service.Name,
            ticket.Status.Name,
            ticket.CreatedAt,
            ticket.StartedAt,
            ticket.EndedAt
        );
    }

    private DateOnly GetQueueDate()
    {
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, _queueTimeZone);
        return DateOnly.FromDateTime(localNow);
    }

    private async Task<Guid> GetActiveSessionIdAsync(
        Guid operatorId, CancellationToken cancellationToken)
    {
        var sessionId = await _context.OperatorSessions
            .AsNoTracking()
            .Where(session =>
                session.OperatorId == operatorId &&
                session.EndedAt == null)
            .Select(session => (Guid?)session.Id)
            .SingleOrDefaultAsync(cancellationToken);
        return sessionId ?? throw new NotFoundException(
            "Not found active operator's session.");
    }
}