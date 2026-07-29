using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Constants;
using QRQueueDecanat.Extensions;
using QRQueueDecanat.Interfaces;
using QRQueueDecanat.Services;

namespace QRQueueDecanat.Controllers;

[Authorize(Roles = "operator")]
[ApiController]
[Route("api/operator")]
public class OperatorSessionsController : ControllerBase
{
    private readonly IOperatorSessionService _operatorSessionService;

    public OperatorSessionsController(IOperatorSessionService operatorSessionService)
    {
        _operatorSessionService = operatorSessionService;
    }

    [HttpPost("session")]
    public async Task<ActionResult<OperatorSessionResponse>> StartSession(
        StartOperatorSessionRequest request, CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        var response = await _operatorSessionService.StartSessionAsync(
            operatorId, request, cancellationToken);
        return Ok(response);
    }
    [HttpDelete("session")]
    public async Task<IActionResult> CloseSession(
        CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        await _operatorSessionService.CloseSessionAsync(
            operatorId, cancellationToken);
        return NoContent();
    }
    [HttpGet("workspace")]
    public async Task<ActionResult<OperatorWorkspaceResponse>> GetWorkspace(
        CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        return Ok(await _operatorSessionService.GetWorkspaceAsync(
            operatorId, cancellationToken));
    }   
}