using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;
using QRQueueDecanat.Entities;
using QRQueueDecanat.Exceptions;
using QRQueueDecanat.Interfaces;

namespace QRQueueDecanat.Services;

public class OperatorSettingsService : IOperatorSettingsService
{
    private readonly ApplicationDbContext _context;

    public OperatorSettingsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<OperatorServiceResponse>> GetOperatorServicesAsync(
        Guid operatorId, CancellationToken cancellationToken = default)
    {
        var selectedServiceIds = _context.OperatorServices
            .Where(operatorService =>
                operatorService.OperatorId == operatorId)
            .Select(operatorService => operatorService.ServiceId);
        return await _context.Services
            .AsNoTracking()
            .Where(service => service.IsActive)
            .OrderBy(service => service.Name)
            .Select(service => new OperatorServiceResponse(
                service.Id,
                service.Name,
                service.Prefix,
                service.IconKey,
                selectedServiceIds.Contains(service.Id)
            ))
            .ToListAsync(cancellationToken);
    }

    public async Task UpdateOperatorServicesAsync(
        Guid operatorId, UpdateOperatorServicesRequest request,
        CancellationToken cancellationToken = default)
    {
        var requestedServiceIds = request.ServiceIds
            .Distinct()
            .ToHashSet();
        var existingServiceCount = await _context.Services
            .CountAsync(service =>
                service.IsActive && 
                requestedServiceIds.Contains(service.Id),
                cancellationToken);
        if (existingServiceCount != requestedServiceIds.Count)
        {
            throw new NotFoundException(
                "One or more services are missing or inactive.");
        }
        var currentServices = await _context.OperatorServices
            .Where(operatorService =>
                operatorService.OperatorId == operatorId)
            .ToListAsync(cancellationToken);
        var currentServiceIds = currentServices
            .Select(operatorService => operatorService.ServiceId)
            .ToHashSet();
        var servicesToRemove = currentServices
            .Where(operatorService =>
                !requestedServiceIds.Contains(operatorService.ServiceId))
            .ToList();
        var servicesToAdd = requestedServiceIds
            .Except(currentServiceIds)
            .Select(serviceId =>
                new OperatorService
                {
                    OperatorId = operatorId,
                    ServiceId = serviceId
                })
            .ToList();
        _context.OperatorServices.RemoveRange(servicesToRemove);
        await _context.OperatorServices.AddRangeAsync(
            servicesToAdd, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }
}