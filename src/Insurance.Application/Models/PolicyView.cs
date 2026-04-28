namespace Insurance.Application.Models;

public sealed record PolicyView(
    Guid PolicyId,
    Guid ApplicationId,
    Guid CustomerId,
    string CoverageType,
    decimal CoverageAmount,
    string Currency,
    int RiskScore,
    DateOnly EffectiveOn,
    DateTimeOffset IssuedOnUtc);
