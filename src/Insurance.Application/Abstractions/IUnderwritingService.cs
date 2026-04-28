using Insurance.Domain;
using Insurance.Application.Models;

namespace Insurance.Application.Abstractions;

public interface IUnderwritingService
{
    Task<UnderwritingDecision> EvaluateAsync(
        PolicyApplication application,
        CancellationToken cancellationToken = default);
}
