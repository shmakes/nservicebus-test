using NServiceBus;

namespace Insurance.Infrastructure.Messaging;

public static class NServiceBusConfig
{
    public static RoutingSettings<LearningTransport> ApplyCommonConfiguration(EndpointConfiguration endpointConfiguration)
    {
        var transport = endpointConfiguration.UseTransport(new LearningTransport());
        endpointConfiguration.UseSerialization<SystemJsonSerializer>();
        endpointConfiguration.UsePersistence<LearningPersistence>();
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo("error");
        endpointConfiguration.AuditProcessedMessagesTo("audit");

        var recoverability = endpointConfiguration.Recoverability();
        recoverability.Immediate(immediate => immediate.NumberOfRetries(2));
        recoverability.Delayed(delayed =>
        {
            delayed.NumberOfRetries(2);
            delayed.TimeIncrease(TimeSpan.FromSeconds(5));
        });

        return transport;
    }
}
