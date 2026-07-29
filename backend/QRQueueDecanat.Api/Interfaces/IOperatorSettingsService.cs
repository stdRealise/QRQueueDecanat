using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IOperatorSettingsService
{
    Task<List<OperatorServiceResponse>> GetOperatorServicesAsync(
        Guid operatorId, CancellationToken cancellationToken = default);
    Task UpdateOperatorServicesAsync(Guid operatorId, 
        UpdateOperatorServicesRequest request,
        CancellationToken cancellationToken = default);
}