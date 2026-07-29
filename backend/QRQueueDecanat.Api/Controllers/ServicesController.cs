using Microsoft.AspNetCore.Mvc;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Services;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Controllers;

[ApiController]
[Route("api/services")]
public class ServicesController : ControllerBase
{
    private readonly IServiceCatalogService _serviceCatalog;

    public ServicesController(IServiceCatalogService serviceCatalog)
    {
        _serviceCatalog = serviceCatalog;
    }
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ServiceResponse>>> Get(
        CancellationToken cancellationToken)
    {
        var services = await _serviceCatalog.GetActiveServicesAsync(cancellationToken);
        return Ok(services);
    }
}