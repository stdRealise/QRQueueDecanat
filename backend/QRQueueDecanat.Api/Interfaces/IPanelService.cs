using Microsoft.EntityFrameworkCore;
using QRQueueDecanat.Data;
using QRQueueDecanat.DTOs;

namespace QRQueueDecanat.Interfaces;

public interface IPanelService
{
    Task<PanelResponse> GetPanelAsync(
        CancellationToken cancellationToken = default);
}