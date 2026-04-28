namespace Insurance.Domain;

public class Policy
{
    public PolicyId Id { get; private set; }
    public PolicyApplicationId ApplicationId { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public CoverageType CoverageType { get; private set; }
    public Money CoverageAmount { get; private set; }
    public RiskScore RiskScore { get; private set; }
    public DateOnly EffectiveOn { get; private set; }
    public DateTimeOffset IssuedOnUtc { get; private set; }

    private Policy()
    {
    }

    private Policy(
        PolicyId id,
        PolicyApplicationId applicationId,
        CustomerId customerId,
        CoverageType coverageType,
        Money coverageAmount,
        RiskScore riskScore,
        DateOnly effectiveOn,
        DateTimeOffset issuedOnUtc)
    {
        Id = id;
        ApplicationId = applicationId;
        CustomerId = customerId;
        CoverageType = coverageType;
        CoverageAmount = coverageAmount;
        RiskScore = riskScore;
        EffectiveOn = effectiveOn;
        IssuedOnUtc = issuedOnUtc;
    }

    public static Policy Issue(
        PolicyId id,
        PolicyApplicationId applicationId,
        CustomerId customerId,
        CoverageType coverageType,
        Money coverageAmount,
        RiskScore riskScore,
        DateOnly effectiveOn,
        DateTimeOffset issuedOnUtc) =>
        new(id, applicationId, customerId, coverageType, coverageAmount, riskScore, effectiveOn, issuedOnUtc);
}
