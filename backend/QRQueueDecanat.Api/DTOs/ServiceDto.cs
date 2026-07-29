namespace QRQueueDecanat.DTOs;

public record ServiceResponse(
    Guid Id,
    string Name,
    string Prefix,
    int? Minutes,
    string? IconKey
);