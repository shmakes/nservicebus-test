using Insurance.Application.Abstractions;
using Insurance.Application.Models;
using Insurance.Domain;
using Insurance.Infrastructure.Repositories;
using Insurance.Messages;
using Insurance.Workflows.Handlers;
using NServiceBus.Testing;
using Xunit;

namespace Insurance.Workflows.Tests;

public sealed class CancellationRegressionTests
{
    [Fact]
    public async Task PerformUnderwriting_Should_Not_Run_When_Application_Was_Cancelled_Before_Delayed_Command()
    {
        var repository = new InMemoryPolicyApplicationRepository();
        var submitHandler = new SubmitPolicyApplicationHandler(repository);
        var cancelHandler = new CancelPolicyApplicationHandler(repository);
        var performHandler = new PerformUnderwritingHandler(repository, new ThrowIfCalledUnderwritingService());

        var applicationId = Guid.NewGuid();

        await submitHandler.Handle(new SubmitPolicyApplication
        {
            ApplicationId = applicationId,
            CustomerId = Guid.NewGuid(),
            CoverageType = "Auto",
            RequestedAmount = 120000m,
            Currency = "USD"
        }, new TestableMessageHandlerContext());

        await cancelHandler.Handle(new CancelPolicyApplication
        {
            ApplicationId = applicationId,
            Reason = "Customer cancelled before underwriting."
        }, new TestableMessageHandlerContext());

        var performContext = new TestableMessageHandlerContext();
        await performHandler.Handle(new PerformUnderwriting
        {
            ApplicationId = applicationId
        }, performContext);

        Assert.Empty(performContext.PublishedMessages);

        var application = await repository.GetAsync(new PolicyApplicationId(applicationId));
        Assert.NotNull(application);
        Assert.Equal(PolicyApplicationStatus.Cancelled, application!.Status);
    }

    private sealed class ThrowIfCalledUnderwritingService : IUnderwritingService
    {
        public Task<UnderwritingDecision> EvaluateAsync(
            PolicyApplication application,
            CancellationToken cancellationToken = default)
        {
            throw new InvalidOperationException("Underwriting should not execute for cancelled applications.");
        }
    }
}
