using Insurance.Domain;

namespace Insurance.Application.Abstractions;

public interface IPolicyApplicationRepository
{
    Task AddAsync(PolicyApplication application, CancellationToken cancellationToken = default);
    Task<PolicyApplication?> GetAsync(PolicyApplicationId id, CancellationToken cancellationToken = default);
    Task UpdateAsync(PolicyApplication application, CancellationToken cancellationToken = default);
}
