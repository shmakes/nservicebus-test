using Insurance.Api.Contracts;
using Insurance.Application.Abstractions;
using Insurance.Domain;
using Insurance.Messages;
using Microsoft.AspNetCore.Mvc;
using NServiceBus;
using System.Text.RegularExpressions;

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

    [HttpGet("examples")]
    [ProducesResponseType(typeof(PolicyApiExamplesResponse), StatusCodes.Status200OK)]
    public IActionResult GetExamples()
    {
        return Ok(new PolicyApiExamplesResponse());
    }

    [HttpPost("applications")]
    [ProducesResponseType(typeof(object), StatusCodes.Status202Accepted)]
    public async Task<IActionResult> SubmitApplication(
        [FromBody] SubmitPolicyApplicationRequest request,
        CancellationToken cancellationToken)
    {
        if (!TryNormalizeAndValidateRequest(request, out var normalizedCoverageType, out var normalizedCurrency, out var validationError))
        {
            return BadRequest(new
            {
                error = validationError,
                allowedCoverageTypes = Enum.GetNames<CoverageType>(),
                allowedCurrencies = new[] { "USD", "CAD", "EUR", "GBP" }
            });
        }

        var applicationId = Guid.NewGuid();

        await messageSession.Send(new SubmitPolicyApplication
        {
            ApplicationId = applicationId,
            CustomerId = request.CustomerId,
            CoverageType = normalizedCoverageType!,
            RequestedAmount = request.RequestedAmount,
            Currency = normalizedCurrency!
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

    private static bool TryNormalizeAndValidateRequest(
        SubmitPolicyApplicationRequest request,
        out string? normalizedCoverageType,
        out string? normalizedCurrency,
        out string? error)
    {
        normalizedCoverageType = null;
        normalizedCurrency = null;
        error = null;

        if (!Enum.TryParse<CoverageType>(request.CoverageType, ignoreCase: true, out var coverageType))
        {
            error = "Invalid coverageType. Use one of: Auto, Home, Life, Health.";
            return false;
        }

        normalizedCoverageType = coverageType.ToString();

        if (string.IsNullOrWhiteSpace(request.Currency))
        {
            error = "Currency is required.";
            return false;
        }

        var currency = request.Currency.Trim().ToUpperInvariant();
        if (!Regex.IsMatch(currency, "^[A-Z]{3}$"))
        {
            error = "Currency must be a 3-letter ISO-style code, for example USD.";
            return false;
        }

        var allowedCurrencies = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "USD", "CAD", "EUR", "GBP"
        };

        if (!allowedCurrencies.Contains(currency))
        {
            error = "Unsupported currency. Use one of: USD, CAD, EUR, GBP.";
            return false;
        }

        normalizedCurrency = currency;
        return true;
    }
}
