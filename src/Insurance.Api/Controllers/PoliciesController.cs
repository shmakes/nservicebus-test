using Insurance.Api.Contracts;
using Insurance.Application.Abstractions;
using Insurance.Messages;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;

namespace Insurance.Api.Controllers;

[ApiController]
[Route("api/policies")]
public sealed class PoliciesController : ControllerBase
{
    private readonly IMessageSession messageSession;
    private readonly IApplicationReadStore applicationReadStore;
    private readonly IPolicyReadStore policyReadStore;

    public PoliciesController(
        IMessageSession messageSession,
        IApplicationReadStore applicationReadStore,
        IPolicyReadStore policyReadStore)
    {
        this.messageSession = messageSession;
        this.applicationReadStore = applicationReadStore;
        this.policyReadStore = policyReadStore;
    }

    [HttpPost("applications")]
    [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SubmitApplication(
        [FromBody] SubmitPolicyApplicationRequest request,
        CancellationToken cancellationToken)
    {
        var applicationId = Guid.NewGuid();

        await messageSession.Send(new SubmitPolicyApplication
        {
            ApplicationId = applicationId,
            CustomerId = request.CustomerId,
            CoverageType = request.CoverageType,
            RequestedAmount = request.RequestedAmount,
            Currency = request.Currency
        }, cancellationToken);

        return AcceptedAtAction(
            nameof(GetApplication),
            new { id = applicationId },
            new { applicationId });
    }

    [HttpGet("applications/{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetApplication(Guid id, CancellationToken cancellationToken)
    {
        var view = await applicationReadStore.GetAsync(id, cancellationToken);
        return view is null ? NotFound() : Ok(view);
    }

    [HttpPost("applications/{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    public async Task<IActionResult> CancelApplication(
        Guid id,
        [FromBody] CancelPolicyApplicationRequest request,
        CancellationToken cancellationToken)
    {
        await messageSession.Send(new CancelPolicyApplication
        {
            ApplicationId = id,
            Reason = request.Reason
        }, cancellationToken);

        return Accepted(new { applicationId = id });
    }

    [HttpGet("{policyId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetPolicy(Guid policyId, CancellationToken cancellationToken)
    {
        var view = await policyReadStore.GetAsync(policyId, cancellationToken);
        return view is null ? NotFound() : Ok(view);
    }
}
