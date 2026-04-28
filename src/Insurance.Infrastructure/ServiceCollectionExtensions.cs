using Insurance.Application.Abstractions;
using Insurance.Infrastructure.ReadStores;
using Insurance.Infrastructure.Repositories;
using Insurance.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Insurance.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInsuranceInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<IPolicyApplicationRepository, InMemoryPolicyApplicationRepository>();
        services.AddSingleton<IPolicyRepository, InMemoryPolicyRepository>();
        services.AddSingleton<IUnderwritingService, RuleBasedUnderwritingService>();
        services.AddSingleton<IApplicationReadStore, InMemoryApplicationReadStore>();
        services.AddSingleton<IPolicyReadStore, InMemoryPolicyReadStore>();
        return services;
    }
}
