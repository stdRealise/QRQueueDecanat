using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Extensions;
using QRQueueDecanat.Interfaces;
using QRQueueDecanat.Services;

namespace QRQueueDecanat.Controllers;

[Authorize(Roles = "operator")]
[ApiController]
[Route("api/operator/settings")]
public class OperatorSettingsController : ControllerBase
{
    private readonly IOperatorSettingsService _operatorSettingsService;

    public OperatorSettingsController(IOperatorSettingsService operatorSettingsService)
    {
        _operatorSettingsService = operatorSettingsService;
    }

    [HttpGet("services")]
    public async Task<ActionResult<List<OperatorServiceResponse>>> GetServices(
        CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        return Ok(await _operatorSettingsService.GetOperatorServicesAsync(
            operatorId, cancellationToken));
    }

    [HttpPut("services")]
    public async Task<IActionResult> UpdateServices(
        UpdateOperatorServicesRequest request,
        CancellationToken cancellationToken)
    {
        var operatorId = User.GetUserId();
        await _operatorSettingsService.UpdateOperatorServicesAsync(
            operatorId, request, cancellationToken);
        return NoContent();
    }
}