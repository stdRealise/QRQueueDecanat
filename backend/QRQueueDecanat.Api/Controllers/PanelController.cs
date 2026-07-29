using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Services;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Controllers;

[ApiController]
[Route("api/panel")]
public class PanelController : ControllerBase
{
    private readonly IPanelService _panelService;

    public PanelController(IPanelService panelService)
    {
        _panelService = panelService;
    }

    [HttpGet]
    public async Task<ActionResult<PanelResponse>> Get(
        CancellationToken cancellationToken)
    {
        return Ok(await _panelService.GetPanelAsync(cancellationToken));
    }
}