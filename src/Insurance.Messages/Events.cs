using NServiceBus;

namespace Insurance.Messages;

public sealed record PolicyApplicationSubmitted : IEvent
{
    public Guid ApplicationId { get; init; }
    public Guid CustomerId { get; init; }
    public string CoverageType { get; init; } = string.Empty;
    public decimal RequestedAmount { get; init; }
    public string Currency { get; init; } = "USD";
    public DateTimeOffset SubmittedOnUtc { get; init; }
}

public sealed record UnderwritingCompleted : IEvent
{
    public Guid ApplicationId { get; init; }
    public bool Approved { get; init; }
    public int RiskScore { get; init; }
    public string? Reason { get; init; }
}

public sealed record PolicyIssued : IEvent
{
    public Guid PolicyId { get; init; }
    public Guid ApplicationId { get; init; }
    public Guid CustomerId { get; init; }
    public string CoverageType { get; init; } = string.Empty;
    public decimal CoverageAmount { get; init; }
    public string Currency { get; init; } = "USD";
    public int RiskScore { get; init; }
    public DateOnly EffectiveOn { get; init; }
    public DateTimeOffset IssuedOnUtc { get; init; }
}

public sealed record PolicyApplicationRejected : IEvent
{
    public Guid ApplicationId { get; init; }
    public string Reason { get; init; } = string.Empty;
}

public sealed record PolicyApplicationCancelled : IEvent
{
    public Guid ApplicationId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
