using System.Collections.Concurrent;
using Insurance.Application.Abstractions;
using Insurance.Domain;

namespace Insurance.Infrastructure.Repositories;

public sealed class InMemoryPolicyApplicationRepository : IPolicyApplicationRepository
{
    private readonly ConcurrentDictionary<Guid, PolicyApplication> storage = new();

    public Task AddAsync(PolicyApplication application, CancellationToken cancellationToken = default)
    {
        storage[application.Id.Value] = application;
        return Task.CompletedTask;
    }

    public Task<PolicyApplication?> GetAsync(PolicyApplicationId id, CancellationToken cancellationToken = default)
    {
        storage.TryGetValue(id.Value, out var application);
        return Task.FromResult(application);
    }

    public Task UpdateAsync(PolicyApplication application, CancellationToken cancellationToken = default)
    {
        storage[application.Id.Value] = application;
        return Task.CompletedTask;
    }
}
