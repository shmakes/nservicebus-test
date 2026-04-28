using Insurance.Application.Abstractions;
using Insurance.Domain;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Workflows.Handlers;

public sealed class CancelPolicyApplicationHandler : IHandleMessages<CancelPolicyApplication>
{
    private readonly IPolicyApplicationRepository repository;

    public CancelPolicyApplicationHandler(IPolicyApplicationRepository repository)
    {
        this.repository = repository;
    }

    public async Task Handle(CancelPolicyApplication message, IMessageHandlerContext context)
    {
        var applicationId = new PolicyApplicationId(message.ApplicationId);
        var application = await repository.GetAsync(applicationId, context.CancellationToken);
        if (application is null)
        {
            return;
        }

        if (application.Status is PolicyApplicationStatus.Cancelled or PolicyApplicationStatus.Issued)
        {
            return;
        }

        application.Cancel(message.Reason);
        await repository.UpdateAsync(application, context.CancellationToken);
    }
}
