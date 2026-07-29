using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Extensions;
using QRQueueDecanat.Interfaces;
using QRQueueDecanat.Services;

namespace QRQueueDecanat.Controllers;

[Authorize(Roles = "operator")]
[ApiController]
[Route("api/operator/tickets")]
public class OperatorTicketController : ControllerBase
{
    private readonly IOperatorTicketsService _operatorTicketsService;

    public OperatorTicketController(IOperatorTicketsService operatorTicketsService)
    {
        _operatorTicketsService = operatorTicketsService;
    }

    [HttpPost("call-next")]
    public async Task<ActionResult<OperatorTicketResponse>> CallNext(
        CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        var response = await _operatorTicketsService.CallNextAsync(
            operatorId, cancellationToken);
        if (response is null)
        {
            return NoContent();
        }
        return Ok(response);
    }

    [HttpPost("{ticketId:guid}/start")]
    public async Task<ActionResult<OperatorTicketResponse>> StartTicket(
        Guid ticketId, CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        var response =
            await _operatorTicketsService.StartTicketAsync(
                operatorId, ticketId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{ticketId:guid}/complete")]
    public async Task<ActionResult<OperatorTicketResponse>> CompleteTicket(
        Guid ticketId, CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        var response = await _operatorTicketsService.CompleteTicketAsync(
            operatorId, ticketId, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{ticketId:guid}/skip")]
    public async Task<ActionResult<OperatorTicketResponse>> SkipTicket(
        Guid ticketId, CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        var response = await _operatorTicketsService.SkipTicketAsync(
            operatorId, ticketId, cancellationToken);
        return Ok(response);
    }

    [HttpGet("history")]
    public async Task<ActionResult<List<OperatorTicketResponse>>> GetHistory(
        CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        var response = await _operatorTicketsService.GetHistoryAsync(
            operatorId, cancellationToken);
        return Ok(response);
    }
}