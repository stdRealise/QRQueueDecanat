using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class PanelService : IPanelService
{
    private readonly ApplicationDbContext _context;
    private readonly TimeZoneInfo _queueTimeZone;

    public PanelService(ApplicationDbContext context,
        TimeZoneInfo queueTimeZone)
    {
        _context = context;
        _queueTimeZone = queueTimeZone;
    }

    public async Task<PanelResponse> GetPanelAsync(
        CancellationToken cancellationToken = default)
    {
        var utcNow = DateTime.UtcNow;
        var localNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _queueTimeZone);
        var queueDate = DateOnly.FromDateTime(localNow);

        var workingCounters = await _context.OperatorSessions
            .CountAsync(session =>
                session.EndedAt == null &&
                session.Status.Name == "work",
                cancellationToken);
        
        var ticketCounts = await _context.Tickets
            .Where(ticket => ticket.ServiceDate == queueDate && 
                (ticket.Status.Name == "waiting" || ticket.Status.Name == "serving"))
            .GroupBy(ticket => ticket.Status.Name)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count, cancellationToken);
        var waitingCount = ticketCounts.GetValueOrDefault("waiting", 0);
        var servingCount = ticketCounts.GetValueOrDefault("serving", 0);
        
        var calls = await _context.Tickets
            .AsNoTracking()
            .Where(ticket =>
                ticket.ServiceDate == queueDate &&
                ticket.SessionId != null &&
                ticket.Status.Name == "called")
            .OrderByDescending(ticket => ticket.CalledAt)
            .Take(8)
            .Select(ticket => new PanelTicketResponse(
                ticket.Id,
                ticket.DisplayNumber,
                ticket.Session.Counter.Number))
            .ToListAsync(cancellationToken);
        var waitingTickets = await _context.Tickets
            .AsNoTracking()
            .Where(ticket =>
                ticket.ServiceDate == queueDate &&
                ticket.Status.Name == "waiting")
            .OrderBy(ticket => ticket.CreatedAt)
            .Take(5)
            .Select(ticket => ticket.DisplayNumber)
            .ToListAsync(cancellationToken);

        var averageWaitingMinutesDouble = await _context.Tickets
            .Where(ticket =>
                ticket.ServiceDate == queueDate &&
                ticket.CalledAt != null)
            .Select(ticket => (double?)(ticket.CalledAt!.Value - ticket.CreatedAt).TotalMinutes)
            .AverageAsync(cancellationToken) ?? 0;
        var averageWaitingMinutes = (int)Math.Round(averageWaitingMinutesDouble);

        return new PanelResponse(
            workingCounters,
            waitingCount,
            servingCount,
            averageWaitingMinutes,
            calls,
            waitingTickets
        );
    }
}