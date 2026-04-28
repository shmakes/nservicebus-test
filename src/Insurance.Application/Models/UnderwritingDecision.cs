using Insurance.Domain;

namespace Insurance.Application.Models;

public sealed record UnderwritingDecision(
    bool Approved,
    RiskScore RiskScore,
    string? Reason);
