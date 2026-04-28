using Insurance.Domain;
using Insurance.Infrastructure.Services;
using Xunit;

namespace Insurance.Workflows.Tests;

public sealed class RuleBasedUnderwritingServiceTests
{
    [Fact]
    public async Task RuleBasedUnderwritingService_Rejects_Life_Over_250k()
    {
        var service = new RuleBasedUnderwritingService();
        var application = PolicyApplication.Submit(
            PolicyApplicationId.New(),
            new CustomerId(Guid.NewGuid()),
            CoverageType.Life,
            new Money(300000m, "USD"),
            DateTimeOffset.UtcNow);

        var result = await service.EvaluateAsync(application);

        Assert.False(result.Approved);
        Assert.Equal(82, result.RiskScore.Value);
        Assert.Contains("manual review", result.Reason!, StringComparison.OrdinalIgnoreCase);
    }
}
