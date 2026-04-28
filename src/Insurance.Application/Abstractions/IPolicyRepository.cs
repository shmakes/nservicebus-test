using Insurance.Domain;

namespace Insurance.Application.Abstractions;

public interface IPolicyRepository
{
    Task AddAsync(Policy policy, CancellationToken cancellationToken = default);
    Task<Policy?> GetAsync(PolicyId policyId, CancellationToken cancellationToken = default);
}
