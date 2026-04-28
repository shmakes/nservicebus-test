using Insurance.Messages;
using Insurance.Workflows.Sagas;
using NServiceBus.Testing;
using Xunit;

namespace Insurance.Workflows.Tests;

public sealed class PolicyApplicationSagaTests
{
    [Fact]
    public async Task PolicyApplicationSaga_Should_Send_PerformUnderwriting_When_Started()
    {
        var saga = new PolicyApplicationSaga();
        var context = new TestableMessageHandlerContext();
        var applicationId = Guid.NewGuid();

        await saga.Handle(new SubmitPolicyApplication
        {
            ApplicationId = applicationId,
            CustomerId = Guid.NewGuid(),
            CoverageType = "Auto",
            RequestedAmount = 120000m,
            Currency = "USD"
        }, context);

        Assert.Contains(context.SentMessages, sent =>
            sent.Message is PerformUnderwriting command &&
            command.ApplicationId == applicationId);
    }

    [Fact]
    public async Task PolicyApplicationSaga_Should_Issue_Policy_When_Underwriting_Approved()
    {
        var saga = new PolicyApplicationSaga();
        var context = new TestableMessageHandlerContext();
        var applicationId = Guid.NewGuid();

        await saga.Handle(new SubmitPolicyApplication
        {
            ApplicationId = applicationId,
            CustomerId = Guid.NewGuid(),
            CoverageType = "Home",
            RequestedAmount = 250000m,
            Currency = "USD"
        }, context);

        await saga.Handle(new UnderwritingCompleted
        {
            ApplicationId = applicationId,
            Approved = true,
            RiskScore = 34
        }, context);

        Assert.Contains(context.SentMessages, sent =>
            sent.Message is IssuePolicy command &&
            command.ApplicationId == applicationId &&
            command.RiskScore == 34);
    }

    [Fact]
    public async Task PolicyApplicationSaga_Should_Publish_Rejected_When_Underwriting_Declined()
    {
        var saga = new PolicyApplicationSaga();
        var context = new TestableMessageHandlerContext();
        var applicationId = Guid.NewGuid();

        await saga.Handle(new SubmitPolicyApplication
        {
            ApplicationId = applicationId,
            CustomerId = Guid.NewGuid(),
            CoverageType = "Life",
            RequestedAmount = 300000m,
            Currency = "USD"
        }, context);

        await saga.Handle(new UnderwritingCompleted
        {
            ApplicationId = applicationId,
            Approved = false,
            RiskScore = 90,
            Reason = "Manual underwriting declined."
        }, context);

        Assert.Contains(context.PublishedMessages, published =>
            published.Message is PolicyApplicationRejected rejected &&
            rejected.ApplicationId == applicationId);
    }
}
