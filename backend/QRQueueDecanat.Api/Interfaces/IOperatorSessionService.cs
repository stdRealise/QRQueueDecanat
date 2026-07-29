using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IOperatorSessionService
{
    Task<OperatorSessionResponse> StartSessionAsync(
        Guid operatorId, StartOperatorSessionRequest request,
        CancellationToken cancellationToken = default);
    Task CloseSessionAsync(Guid operatorId, 
        CancellationToken cancellationToken = default);
    Task<OperatorWorkspaceResponse> GetWorkspaceAsync(
        Guid operatorId, CancellationToken cancellationToken = default);
}