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

        var timestamp = DateTimeOffset.UtcNow;
        var updatedTimeline = new Dictionary<string, DateTimeOffset>(current.Timeline)
        {
            ["Cancelled"] = timestamp
        };

        await readStore.UpsertAsync(
            current with
            {
                Status = "Cancelled",
                Reason = message.Reason,
                UpdatedOnUtc = timestamp,
                Timeline = updatedTimeline
            },
            context.CancellationToken);
    }
}
