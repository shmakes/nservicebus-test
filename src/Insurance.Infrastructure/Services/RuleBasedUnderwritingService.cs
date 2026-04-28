using Insurance.Application.Abstractions;
using Insurance.Application.Models;
using Insurance.Domain;

namespace Insurance.Infrastructure.Services;

public sealed class RuleBasedUnderwritingService : IUnderwritingService
{
    public Task<UnderwritingDecision> EvaluateAsync(
        PolicyApplication application,
        CancellationToken cancellationToken = default)
    {
        var amount = application.RequestedCoverage.Amount;
        var coverageType = application.CoverageType;

        if (amount > 1_000_000m)
        {
            return Task.FromResult(new UnderwritingDecision(
                Approved: false,
                RiskScore: new RiskScore(95),
                Reason: "Requested amount exceeds allowed threshold."));
        }

        if (coverageType == CoverageType.Life && amount > 250_000m)
        {
            return Task.FromResult(new UnderwritingDecision(
                Approved: false,
                RiskScore: new RiskScore(82),
                Reason: "Life coverage over 250000 requires manual review."));
        }

        var calculatedScore = coverageType switch
        {
            CoverageType.Auto => 35,
            CoverageType.Home => 30,
            CoverageType.Life => 55,
            CoverageType.Health => 45,
            _ => 50
        };

        return Task.FromResult(new UnderwritingDecision(
            Approved: true,
            RiskScore: new RiskScore(calculatedScore),
            Reason: null));
    }
}
