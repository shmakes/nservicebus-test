using System.Collections.Concurrent;
using Insurance.Application.Abstractions;
using Insurance.Application.Models;

namespace Insurance.Infrastructure.ReadStores;

public sealed class InMemoryPolicyReadStore : IPolicyReadStore
{
    private readonly ConcurrentDictionary<Guid, PolicyView> storage = new();

    public Task UpsertAsync(PolicyView view, CancellationToken cancellationToken = default)
    {
        storage[view.PolicyId] = view;
        return Task.CompletedTask;
    }

    public Task<PolicyView?> GetAsync(Guid policyId, CancellationToken cancellationToken = default)
    {
        storage.TryGetValue(policyId, out var view);
        return Task.FromResult(view);
    }
}
