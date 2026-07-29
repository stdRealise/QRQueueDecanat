using System.Security.Claims;

namespace QRQueueDecanat.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetUserId(this ClaimsPrincipal principal)
    {
        if (principal == null) {
            throw new ArgumentNullException(nameof(principal));
        }
        var userIdValue = principal.FindFirstValue(
            ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("Invalid user id.");
        }
        return userId;
    }
}