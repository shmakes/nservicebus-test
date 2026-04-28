namespace Insurance.Api.Contracts;

public sealed record SubmitPolicyApplicationRequest
{
    public Guid CustomerId { get; init; }
    public string CoverageType { get; init; } = "Auto";
    public decimal RequestedAmount { get; init; }
    public string Currency { get; init; } = "USD";
}
