using Insurance.Application.Abstractions;
using Insurance.Application.Models;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Api.MessageHandlers;

public sealed class PolicyApplicationSubmittedHandler : IHandleMessages<PolicyApplicationSubmitted>
{
    private readonly IApplicationReadStore readStore;

    public PolicyApplicationSubmittedHandler(IApplicationReadStore readStore)
    {
        this.readStore = readStore;
    }

    public Task Handle(PolicyApplicationSubmitted message, IMessageHandlerContext context)
    {
        return readStore.UpsertAsync(new PolicyApplicationView(
            message.ApplicationId,
            message.CustomerId,
            message.CoverageType,
            message.RequestedAmount,
            message.Currency,
            Status: "Submitted",
            RiskScore: null,
            Reason: null,
            UpdatedOnUtc: message.SubmittedOnUtc), context.CancellationToken);
    }
}
