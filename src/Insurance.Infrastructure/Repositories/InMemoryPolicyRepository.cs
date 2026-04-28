using System.Collections.Concurrent;
using Insurance.Application.Abstractions;
using Insurance.Domain;

namespace Insurance.Infrastructure.Repositories;

public sealed class InMemoryPolicyRepository : IPolicyRepository
{
    private readonly ConcurrentDictionary<Guid, Policy> storage = new();

    public Task AddAsync(Policy policy, CancellationToken cancellationToken = default)
    {
        storage[policy.Id.Value] = policy;
        return Task.CompletedTask;
    }

    public Task<Policy?> GetAsync(PolicyId policyId, CancellationToken cancellationToken = default)
    {
        storage.TryGetValue(policyId.Value, out var policy);
        return Task.FromResult(policy);
    }
}
