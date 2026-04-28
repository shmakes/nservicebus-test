using Insurance.Application.Abstractions;
using Insurance.Domain;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Workflows.Handlers;

public sealed class IssuePolicyHandler : IHandleMessages<IssuePolicy>
{
    private readonly IPolicyApplicationRepository applicationRepository;
    private readonly IPolicyRepository policyRepository;

    public IssuePolicyHandler(
        IPolicyApplicationRepository applicationRepository,
        IPolicyRepository policyRepository)
    {
        this.applicationRepository = applicationRepository;
        this.policyRepository = policyRepository;
    }

    public async Task Handle(IssuePolicy message, IMessageHandlerContext context)
    {
        var applicationId = new PolicyApplicationId(message.ApplicationId);
        var application = await applicationRepository.GetAsync(applicationId, context.CancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException($"Policy application '{message.ApplicationId}' was not found.");
        }

        application.Approve(new RiskScore(message.RiskScore));
        application.MarkIssued();
        await applicationRepository.UpdateAsync(application, context.CancellationToken);

        var policyId = PolicyId.New();
        var issuedOnUtc = DateTimeOffset.UtcNow;
        var policy = Policy.Issue(
            policyId,
            application.Id,
            application.CustomerId,
            application.CoverageType,
            application.RequestedCoverage,
            new RiskScore(message.RiskScore),
            DateOnly.FromDateTime(issuedOnUtc.UtcDateTime.Date),
            issuedOnUtc);

        await policyRepository.AddAsync(policy, context.CancellationToken);

        await context.Publish(new PolicyIssued
        {
            PolicyId = policy.Id.Value,
            ApplicationId = application.Id.Value,
            CustomerId = application.CustomerId.Value,
            CoverageType = application.CoverageType.ToString(),
            CoverageAmount = application.RequestedCoverage.Amount,
            Currency = application.RequestedCoverage.Currency,
            RiskScore = message.RiskScore,
            EffectiveOn = policy.EffectiveOn,
            IssuedOnUtc = issuedOnUtc
        });
    }
}
