namespace Insurance.Api.Contracts;

public sealed record CancelPolicyApplicationRequest
{
    public string Reason { get; init; } = "Cancelled by operator.";
}
