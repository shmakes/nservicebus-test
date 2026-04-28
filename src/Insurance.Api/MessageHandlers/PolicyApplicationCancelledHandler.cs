using Insurance.Application.Abstractions;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Api.MessageHandlers;

public sealed class PolicyApplicationCancelledHandler : IHandleMessages<PolicyApplicationCancelled>
{
    private readonly IApplicationReadStore readStore;

    public PolicyApplicationCancelledHandler(IApplicationReadStore readStore)
    {
        this.readStore = readStore;
    }

    public async Task Handle(PolicyApplicationCancelled message, IMessageHandlerContext context)
    {
        var current = await readStore.GetAsync(message.ApplicationId, context.CancellationToken);
        if (current is null)
        {
            return;
        }

        await readStore.UpsertAsync(
            current with
            {
                Status = "Cancelled",
                Reason = message.Reason,
                UpdatedOnUtc = DateTimeOffset.UtcNow
            },
            context.CancellationToken);
    }
}
