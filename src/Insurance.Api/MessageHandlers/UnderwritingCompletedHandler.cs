using Insurance.Application.Abstractions;
using Insurance.Application.Models;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Api.MessageHandlers;

public sealed class UnderwritingCompletedHandler : IHandleMessages<UnderwritingCompleted>
{
    private readonly IApplicationReadStore readStore;

    public UnderwritingCompletedHandler(IApplicationReadStore readStore)
    {
        this.readStore = readStore;
    }

    public async Task Handle(UnderwritingCompleted message, IMessageHandlerContext context)
    {
        var current = await readStore.GetAsync(message.ApplicationId, context.CancellationToken);
        if (current is null)
        {
            return;
        }

        var updatedStatus = message.Approved ? "Approved" : "Rejected";
        var updatedReason = message.Approved ? null : message.Reason;
        var timestamp = DateTimeOffset.UtcNow;
        var updatedTimeline = new Dictionary<string, DateTimeOffset>(current.Timeline)
        {
            [updatedStatus] = timestamp
        };

        await readStore.UpsertAsync(
            current with
            {
                Status = updatedStatus,
                RiskScore = message.RiskScore,
                Reason = updatedReason,
                UpdatedOnUtc = timestamp,
                Timeline = updatedTimeline
            },
            context.CancellationToken);
    }
}
