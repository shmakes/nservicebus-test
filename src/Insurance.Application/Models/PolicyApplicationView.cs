namespace Insurance.Application.Models;

public sealed record PolicyApplicationView(
    Guid ApplicationId,
    Guid? PolicyId,
    Guid CustomerId,
    string CoverageType,
    decimal RequestedAmount,
    string Currency,
    string Status,
    int? RiskScore,
    string? Reason,
    DateTimeOffset UpdatedOnUtc,
    IReadOnlyDictionary<string, DateTimeOffset> Timeline);
