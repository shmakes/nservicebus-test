using Insurance.Infrastructure;
using Insurance.Infrastructure.Messaging;
using Insurance.Messages;
using NServiceBus;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddInsuranceInfrastructure();

builder.Host.UseNServiceBus(_ =>
{
    var endpointConfiguration = new EndpointConfiguration("Insurance.Api");
    var routing = NServiceBusConfig.ApplyCommonConfiguration(endpointConfiguration);

    routing.RouteToEndpoint(typeof(SubmitPolicyApplication), "Insurance.Workflows");
    routing.RouteToEndpoint(typeof(CancelPolicyApplication), "Insurance.Workflows");

    return endpointConfiguration;
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();
app.MapControllers();

await app.RunAsync();
