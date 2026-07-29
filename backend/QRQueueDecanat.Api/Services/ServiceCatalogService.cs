using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class ServiceCatalogService : IServiceCatalogService
{
    private readonly ApplicationDbContext _context;

    public ServiceCatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceResponse>> GetActiveServicesAsync(
        CancellationToken cancellationToken = default)
    {
        return await _context.Services
            .AsNoTracking()
            .Where(service => service.IsActive)
            .OrderBy(service => service.Name)
            .Select(service => new ServiceResponse(
                service.Id,
                service.Name,
                service.Prefix,
                service.Minutes,
                service.IconKey
            )).ToListAsync(cancellationToken);
    }
}