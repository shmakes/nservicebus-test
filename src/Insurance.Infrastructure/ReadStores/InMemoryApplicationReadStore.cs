using System.Collections.Concurrent;
using Insurance.Application.Abstractions;
using Insurance.Application.Models;

namespace Insurance.Infrastructure.ReadStores;

public sealed class InMemoryApplicationReadStore : IApplicationReadStore
{
    private readonly ConcurrentDictionary<Guid, PolicyApplicationView> storage = new();

    public Task UpsertAsync(PolicyApplicationView view, CancellationToken cancellationToken = default)
    {
        storage[view.ApplicationId] = view;
        return Task.CompletedTask;
    }

    public Task<PolicyApplicationView?> GetAsync(Guid applicationId, CancellationToken cancellationToken = default)
    {
        storage.TryGetValue(applicationId, out var view);
        return Task.FromResult(view);
    }
}
