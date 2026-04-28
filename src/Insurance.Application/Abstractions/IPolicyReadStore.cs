using Insurance.Application.Models;

namespace Insurance.Application.Abstractions;

public interface IPolicyReadStore
{
    Task UpsertAsync(PolicyView view, CancellationToken cancellationToken = default);
    Task<PolicyView?> GetAsync(Guid policyId, CancellationToken cancellationToken = default);
}
