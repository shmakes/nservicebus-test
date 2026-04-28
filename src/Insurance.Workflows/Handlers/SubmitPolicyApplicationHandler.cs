using Insurance.Application.Abstractions;
using Insurance.Domain;
using Insurance.Messages;
using NServiceBus;

namespace Insurance.Workflows.Handlers;

public sealed class SubmitPolicyApplicationHandler : IHandleMessages<SubmitPolicyApplication>
{
    private readonly IPolicyApplicationRepository repository;

    public SubmitPolicyApplicationHandler(IPolicyApplicationRepository repository)
    {
        this.repository = repository;
    }

    public async Task Handle(SubmitPolicyApplication message, IMessageHandlerContext context)
    {
        var coverageType = Enum.Parse<CoverageType>(message.CoverageType, ignoreCase: true);

        var application = PolicyApplication.Submit(
            new PolicyApplicationId(message.ApplicationId),
            new CustomerId(message.CustomerId),
            coverageType,
            new Money(message.RequestedAmount, message.Currency),
            DateTimeOffset.UtcNow);

        await repository.AddAsync(application, context.CancellationToken);

        await context.Publish(new PolicyApplicationSubmitted
        {
            ApplicationId = message.ApplicationId,
            CustomerId = message.CustomerId,
            CoverageType = message.CoverageType,
            RequestedAmount = message.RequestedAmount,
            Currency = message.Currency,
            SubmittedOnUtc = application.SubmittedOnUtc
        });
    }
}
