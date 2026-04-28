using NServiceBus;

namespace Insurance.Messages;

public sealed record SubmitPolicyApplication : ICommand
{
    public Guid ApplicationId { get; init; }
    public Guid CustomerId { get; init; }
    public string CoverageType { get; init; } = string.Empty;
    public decimal RequestedAmount { get; init; }
    public string Currency { get; init; } = "USD";
}

public sealed record PerformUnderwriting : ICommand
{
    public Guid ApplicationId { get; init; }
}

public sealed record IssuePolicy : ICommand
{
    public Guid ApplicationId { get; init; }
    public int RiskScore { get; init; }
}

public sealed record CancelPolicyApplication : ICommand
{
    public Guid ApplicationId { get; init; }
    public string Reason { get; init; } = string.Empty;
}
