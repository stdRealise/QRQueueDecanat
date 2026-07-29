using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IServiceCatalogService
{
    Task<IEnumerable<ServiceResponse>> GetActiveServicesAsync(
        CancellationToken cancellationToken = default);
}