using Insurance.Infrastructure;
using Insurance.Infrastructure.Messaging;
using NServiceBus;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddInsuranceInfrastructure();

var endpointConfiguration = new EndpointConfiguration("Insurance.Workflows");
NServiceBusConfig.ApplyCommonConfiguration(endpointConfiguration);
builder.UseNServiceBus(endpointConfiguration);

await builder.Build().RunAsync();
