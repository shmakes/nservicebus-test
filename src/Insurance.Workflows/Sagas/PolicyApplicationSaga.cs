using Insurance.Messages;
using NServiceBus;

namespace Insurance.Workflows.Sagas;

public sealed class PolicyApplicationSaga :
    Saga<PolicyApplicationSagaData>,
    IAmStartedByMessages<SubmitPolicyApplication>,
    IHandleMessages<UnderwritingCompleted>,
    IHandleMessages<PolicyIssued>,
    IHandleMessages<CancelPolicyApplication>,
    IHandleTimeouts<UnderwritingTimeout>
{
    protected override void ConfigureHowToFindSaga(SagaPropertyMapper<PolicyApplicationSagaData> mapper)
    {
        mapper.MapSaga(saga => saga.ApplicationId)
            .ToMessage<SubmitPolicyApplication>(message => message.ApplicationId)
            .ToMessage<UnderwritingCompleted>(message => message.ApplicationId)
            .ToMessage<PolicyIssued>(message => message.ApplicationId)
            .ToMessage<CancelPolicyApplication>(message => message.ApplicationId);
    }

    public async Task Handle(SubmitPolicyApplication message, IMessageHandlerContext context)
    {
        Data ??= new PolicyApplicationSagaData();

        Data.ApplicationId = message.ApplicationId;
        Data.CustomerId = message.CustomerId;
        Data.CoverageType = message.CoverageType;
        Data.RequestedAmount = message.RequestedAmount;
        Data.Currency = message.Currency;
        Data.Status = "Submitted";

        await context.SendLocal(new PerformUnderwriting
        {
            ApplicationId = message.ApplicationId
        });

        Data.Status = "Underwriting";

        await RequestTimeout(
            context,
            TimeSpan.FromSeconds(30),
            new UnderwritingTimeout
            {
                ApplicationId = message.ApplicationId
            });
    }

    public async Task Handle(UnderwritingCompleted message, IMessageHandlerContext context)
    {
        if (Data.Status is "Cancelled" or "Rejected" or "Issued")
        {
            return;
        }

        Data.RiskScore = message.RiskScore;

        if (!message.Approved)
        {
            Data.Status = "Rejected";
            await context.Publish(new PolicyApplicationRejected
            {
                ApplicationId = message.ApplicationId,
                Reason = message.Reason ?? "Application rejected by underwriting."
            });
            MarkAsComplete();
            return;
        }

        Data.Status = "Approved";
        await context.SendLocal(new IssuePolicy
        {
            ApplicationId = message.ApplicationId,
            RiskScore = message.RiskScore
        });
    }

    public Task Handle(PolicyIssued message, IMessageHandlerContext context)
    {
        Data.Status = "Issued";
        MarkAsComplete();
        return Task.CompletedTask;
    }

    public async Task Handle(CancelPolicyApplication message, IMessageHandlerContext context)
    {
        if (Data.Status is "Issued" or "Cancelled")
        {
            return;
        }

        Data.Status = "Cancelled";
        await context.Publish(new PolicyApplicationCancelled
        {
            ApplicationId = message.ApplicationId,
            Reason = message.Reason
        });
        MarkAsComplete();
    }

    public async Task Timeout(UnderwritingTimeout state, IMessageHandlerContext context)
    {
        if (Data.Status != "Underwriting")
        {
            return;
        }

        Data.Status = "Rejected";
        await context.Publish(new PolicyApplicationRejected
        {
            ApplicationId = state.ApplicationId,
            Reason = "Underwriting timeout reached before completion."
        });
        MarkAsComplete();
    }
}
