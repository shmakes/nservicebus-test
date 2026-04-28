using Insurance.Application.Abstractions;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Api.MessageHandlers;

public sealed class PolicyApplicationRejectedHandler : IHandleMessages<PolicyApplicationRejected>
{
    private readonly IApplicationReadStore readStore;

    public PolicyApplicationRejectedHandler(IApplicationReadStore readStore)
    {
        this.readStore = readStore;
    }

    public async Task Handle(PolicyApplicationRejected message, IMessageHandlerContext context)
    {
        var current = await readStore.GetAsync(message.ApplicationId, context.CancellationToken);
        if (current is null)
        {
            return;
        }

        var timestamp = DateTimeOffset.UtcNow;
        var updatedTimeline = new Dictionary<string, DateTimeOffset>(current.Timeline)
        {
            ["Rejected"] = timestamp
        };

        await readStore.UpsertAsync(
            current with
            {
                Status = "Rejected",
                Reason = message.Reason,
                UpdatedOnUtc = timestamp,
                Timeline = updatedTimeline
            },
            context.CancellationToken);
    }
}
