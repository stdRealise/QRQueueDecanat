using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Constants;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Entities;
using QRQueueDecanat.Exceptions;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class OperatorSessionService : IOperatorSessionService
{
    private readonly ApplicationDbContext _context;
    private readonly TimeZoneInfo _queueTimeZone;

    public OperatorSessionService(ApplicationDbContext context, 
        TimeZoneInfo queueTimeZone)
    {
        _context = context;
        _queueTimeZone = queueTimeZone;
    }

    public async Task<OperatorSessionResponse> StartSessionAsync(
        Guid operatorId, StartOperatorSessionRequest request,
        CancellationToken cancellationToken = default)
    {
        var operatorUser = await _context.Users
            .AsNoTracking()
            .SingleOrDefaultAsync(user =>
                user.Id == operatorId && user.IsActive,
                cancellationToken)
            ?? throw new NotFoundException("Operator not found.");
        var counter = await _context.Counters
            .AsNoTracking()
            .SingleOrDefaultAsync(counter =>
                counter.Number == request.CounterNumber &&
                counter.IsActive, cancellationToken)
            ?? throw new NotFoundException("Counter not found.");

        var activeSession = await _context.OperatorSessions
            .AsNoTracking()
            .Where(session =>
                session.EndedAt == null &&
                session.OperatorId == operatorId)
            .Select(session => new 
            {
                session.CounterId,
                Response = new OperatorSessionResponse(
                    session.Operator.FullName,
                    session.Counter.Number,
                    session.StartedAt)
            })
                .SingleOrDefaultAsync(cancellationToken);
        if (activeSession is not null)
        {
            if (activeSession.CounterId == counter.Id) {
                return activeSession.Response;
            }
            throw new ConflictException(
                $"The operator is already working in {activeSession.Response.CounterNumber} counter.");
        }

        var counterIsBusy = await _context.OperatorSessions
            .AnyAsync(session =>
                session.EndedAt == null &&
                session.CounterId == counter.Id,
                cancellationToken);
        if (counterIsBusy)
        {
            throw new ConflictException(
                "Another operator is working at this counter.");
        }

        var activeStatus = await _context.OperatorStatuses
            .SingleAsync(status => status.Name == StatusNames.Active,
                cancellationToken);
        var session = new OperatorSession
        {
            OperatorId = operatorId,
            CounterId = counter.Id,
            StatusId = activeStatus.Id,
            StartedAt = DateTime.UtcNow
        };
        _context.OperatorSessions.Add(session);
        await _context.SaveChangesAsync(cancellationToken);
        return new OperatorSessionResponse(
            operatorUser.FullName,
            counter.Number,
            session.StartedAt
        );
    }

    public async Task CloseSessionAsync(Guid operatorId,
        CancellationToken cancellationToken = default)
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
            ?? throw new NotFoundException(
                "Session not activated.");

        var hasCurrentTicket = await _context.Tickets
            .AnyAsync(ticket =>
                ticket.SessionId == session.Id &&
                (
                    ticket.Status.Name == StatusNames.Called ||
                    ticket.Status.Name == StatusNames.Serving
                ), cancellationToken);
        if (hasCurrentTicket)
        {
            throw new ConflictException(
                "Finish working with the current ticket first.");
        }
        session.EndedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);
    }

    public async Task<OperatorWorkspaceResponse> GetWorkspaceAsync(
        Guid operatorId, CancellationToken cancellationToken = default)
    {
        var sessionId = await _context.OperatorSessions
            .AsNoTracking()
            .Where(session =>
                session.OperatorId == operatorId &&
                session.EndedAt == null)
            .Select(session => (Guid?)session.Id)
            .SingleOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundException("Session not activated.");
        
        var currentTicket = await _context.Tickets
            .AsNoTracking()
            .Where(ticket =>
                ticket.SessionId == sessionId &&
                (
                    ticket.Status.Name == StatusNames.Called ||
                    ticket.Status.Name == StatusNames.Serving
                ))
            .Select(ticket => new OperatorTicketResponse(
                ticket.Id,
                ticket.DisplayNumber,
                ticket.ServiceId,
                ticket.Service.Name,
                ticket.Status.Name,
                ticket.CreatedAt,
                ticket.StartedAt,
                ticket.EndedAt
            )).SingleOrDefaultAsync(cancellationToken);

        var queueDate = GetQueueDate();
        var operatorServiceIds = _context.OperatorServices
            .Where(service => service.OperatorId == operatorId)
            .Select(service => service.ServiceId);
        var waitingTickets = await _context.Tickets
            .AsNoTracking()
            .Where(ticket =>
                ticket.ServiceDate == queueDate &&
                ticket.Status.Name == StatusNames.Waiting &&
                operatorServiceIds.Contains(ticket.ServiceId))
            .OrderBy(ticket => ticket.CreatedAt)
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
        return new OperatorWorkspaceResponse(
            currentTicket,
            waitingTickets);
    }
    private DateOnly GetQueueDate()
    {
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(
            DateTime.UtcNow, _queueTimeZone);
        return DateOnly.FromDateTime(localNow);
    }
}