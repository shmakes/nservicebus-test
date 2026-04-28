using NServiceBus;

namespace Insurance.Workflows.Sagas;

public sealed class PolicyApplicationSagaData : ContainSagaData
{
    public Guid ApplicationId { get; set; }
    public Guid CustomerId { get; set; }
    public string CoverageType { get; set; } = string.Empty;
    public decimal RequestedAmount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public int? RiskScore { get; set; }
    public string Status { get; set; } = string.Empty;
}
