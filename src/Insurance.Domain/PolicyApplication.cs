namespace Insurance.Domain;

public class PolicyApplication
{
    public PolicyApplicationId Id { get; private set; }
    public CustomerId CustomerId { get; private set; }
    public CoverageType CoverageType { get; private set; }
    public Money RequestedCoverage { get; private set; }
    public PolicyApplicationStatus Status { get; private set; }
    public RiskScore? RiskScore { get; private set; }
    public string? RejectionReason { get; private set; }
    public DateTimeOffset SubmittedOnUtc { get; private set; }

    private PolicyApplication()
    {
    }

    private PolicyApplication(
        PolicyApplicationId id,
        CustomerId customerId,
        CoverageType coverageType,
        Money requestedCoverage,
        DateTimeOffset submittedOnUtc)
    {
        Id = id;
        CustomerId = customerId;
        CoverageType = coverageType;
        RequestedCoverage = requestedCoverage;
        SubmittedOnUtc = submittedOnUtc;
        Status = PolicyApplicationStatus.Submitted;
    }

    public static PolicyApplication Submit(
        PolicyApplicationId id,
        CustomerId customerId,
        CoverageType coverageType,
        Money requestedCoverage,
        DateTimeOffset submittedOnUtc) =>
        new(id, customerId, coverageType, requestedCoverage, submittedOnUtc);

    public void StartUnderwriting()
    {
        EnsureStatusIsOneOf(PolicyApplicationStatus.Submitted);
        Status = PolicyApplicationStatus.Underwriting;
    }

    public void Approve(RiskScore riskScore)
    {
        EnsureStatusIsOneOf(PolicyApplicationStatus.Underwriting);
        RiskScore = riskScore;
        Status = PolicyApplicationStatus.Approved;
        RejectionReason = null;
    }

    public void Reject(string reason)
    {
        EnsureStatusIsOneOf(PolicyApplicationStatus.Underwriting, PolicyApplicationStatus.Submitted);

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Rejection reason is required.", nameof(reason));
        }

        Status = PolicyApplicationStatus.Rejected;
        RejectionReason = reason.Trim();
    }

    public void Cancel(string reason)
    {
        EnsureStatusIsOneOf(
            PolicyApplicationStatus.Submitted,
            PolicyApplicationStatus.Underwriting,
            PolicyApplicationStatus.Approved);

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));
        }

        Status = PolicyApplicationStatus.Cancelled;
        RejectionReason = reason.Trim();
    }

    public void MarkIssued()
    {
        EnsureStatusIsOneOf(PolicyApplicationStatus.Approved);
        Status = PolicyApplicationStatus.Issued;
    }

    private void EnsureStatusIsOneOf(params PolicyApplicationStatus[] allowedStatuses)
    {
        if (allowedStatuses.Contains(Status))
        {
            return;
        }

        throw new InvalidOperationException($"Operation is invalid when application is in '{Status}' status.");
    }
}
