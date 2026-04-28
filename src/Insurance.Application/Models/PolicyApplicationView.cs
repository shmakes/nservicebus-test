namespace Insurance.Application.Models;

public sealed record PolicyApplicationView(
    Guid ApplicationId,
    Guid CustomerId,
    string CoverageType,
    decimal RequestedAmount,
    string Currency,
    string Status,
    int? RiskScore,
    string? Reason,
    DateTimeOffset UpdatedOnUtc);
