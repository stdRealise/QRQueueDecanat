using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Interfaces;
using QRQueueDecanat.Services;

namespace QRQueueDecanat.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    private readonly ITicketService _ticketService;

    public TicketsController(ITicketService ticketService)
    {
        _ticketService = ticketService;
    }

    [HttpPost]
    public async Task<ActionResult<TicketResponse>> Create(
        [FromBody] CreateTicketRequest request,
        CancellationToken cancellationToken)
    {
        var response = await _ticketService.CreateTicketAsync(
            request.ServiceId,cancellationToken);
        return CreatedAtAction(nameof(GetById),
            new { ticketId = response.Id },
            response);
    }

    [HttpGet("{ticketId:guid}")]
    public async Task<ActionResult<TicketResponse>> GetById(
        Guid ticketId, CancellationToken cancellationToken)
    {
        var response = await _ticketService.GetTicketAsync(
            ticketId, cancellationToken);
        if (response is null)
        {
            return NotFound(new ProblemDetails
            {
                Status = StatusCodes.Status404NotFound,
                Title = "Талон не найден",
                Detail = "Талон с указанным идентификатором не существует."
            });
        }
        return Ok(response);
    }

    [HttpPost("{ticketId:guid}/cancel")]
    public async Task<ActionResult<TicketResponse>> Cancel(
        Guid ticketId, CancellationToken cancellationToken)
    {
        var response = await _ticketService.CancelTicketAsync(
            ticketId, cancellationToken);
        return Ok(response);
    }
}