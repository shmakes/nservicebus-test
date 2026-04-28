using Insurance.Application.Abstractions;
using Insurance.Domain;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Workflows.Handlers;

public sealed class PerformUnderwritingHandler : IHandleMessages<PerformUnderwriting>
{
    private readonly IPolicyApplicationRepository repository;
    private readonly IUnderwritingService underwritingService;

    public PerformUnderwritingHandler(
        IPolicyApplicationRepository repository,
        IUnderwritingService underwritingService)
    {
        this.repository = repository;
        this.underwritingService = underwritingService;
    }

    public async Task Handle(PerformUnderwriting message, IMessageHandlerContext context)
    {
        var applicationId = new PolicyApplicationId(message.ApplicationId);
        var application = await repository.GetAsync(applicationId, context.CancellationToken);

        if (application is null)
        {
            throw new InvalidOperationException($"Policy application '{message.ApplicationId}' was not found.");
        }

        if (application.Status is PolicyApplicationStatus.Cancelled or PolicyApplicationStatus.Rejected or PolicyApplicationStatus.Issued)
        {
            return;
        }

        if (application.Status == PolicyApplicationStatus.Submitted)
        {
            application.StartUnderwriting();
            await repository.UpdateAsync(application, context.CancellationToken);
        }

        if (application.Status != PolicyApplicationStatus.Underwriting)
        {
            return;
        }

        var decision = await underwritingService.EvaluateAsync(application, context.CancellationToken);

        await context.Publish(new UnderwritingCompleted
        {
            ApplicationId = message.ApplicationId,
            Approved = decision.Approved,
            RiskScore = decision.RiskScore.Value,
            Reason = decision.Reason
        });
    }
}
