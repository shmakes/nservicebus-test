using Insurance.Application.Abstractions;
using Insurance.Application.Models;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Api.MessageHandlers;

public sealed class PolicyIssuedHandler : IHandleMessages<PolicyIssued>
{
    private readonly IApplicationReadStore applicationReadStore;
    private readonly IPolicyReadStore policyReadStore;

    public PolicyIssuedHandler(IApplicationReadStore applicationReadStore, IPolicyReadStore policyReadStore)
    {
        this.applicationReadStore = applicationReadStore;
        this.policyReadStore = policyReadStore;
    }

    public async Task Handle(PolicyIssued message, IMessageHandlerContext context)
    {
        await policyReadStore.UpsertAsync(new PolicyView(
            message.PolicyId,
            message.ApplicationId,
            message.CustomerId,
            message.CoverageType,
            message.CoverageAmount,
            message.Currency,
            message.RiskScore,
            message.EffectiveOn,
            message.IssuedOnUtc), context.CancellationToken);

        var application = await applicationReadStore.GetAsync(message.ApplicationId, context.CancellationToken);
        if (application is null)
        {
            return;
        }

        await applicationReadStore.UpsertAsync(
            application with
            {
                Status = "Issued",
                RiskScore = message.RiskScore,
                UpdatedOnUtc = message.IssuedOnUtc
            },
            context.CancellationToken);
    }
}
