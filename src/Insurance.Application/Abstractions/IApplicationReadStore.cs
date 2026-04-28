using Insurance.Application.Models;

namespace Insurance.Application.Abstractions;

public interface IApplicationReadStore
{
    Task UpsertAsync(PolicyApplicationView view, CancellationToken cancellationToken = default);
    Task<PolicyApplicationView?> GetAsync(Guid applicationId, CancellationToken cancellationToken = default);
}
