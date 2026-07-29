namespace QRQueueDecanat.DTOs;

public record LoginRequest(
    string Username,
    string Password
);

public record AuthenticatedUserResponse(
    Guid Id,
    string FullName,
    string RoleName
);

public record LoginResponse(
    string AccessToken,
    DateTime ExpiresAtUtc,
    AuthenticatedUserResponse User
);