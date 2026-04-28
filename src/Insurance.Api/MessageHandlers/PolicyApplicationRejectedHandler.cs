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

        await readStore.UpsertAsync(
            current with
            {
                Status = "Rejected",
                Reason = message.Reason,
                UpdatedOnUtc = DateTimeOffset.UtcNow
            },
            context.CancellationToken);
    }
}
