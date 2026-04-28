namespace Insurance.Api.Contracts;

public sealed record PolicyApiExamplesResponse
{
    public string[] AllowedCoverageTypes { get; init; } = ["Auto", "Home", "Life", "Health"];

    public string[] AllowedCurrencies { get; init; } = ["USD", "CAD", "EUR", "GBP"];

    public SubmitPolicyApplicationRequest SubmitAutoPolicyApplication { get; init; } = new()
    {
        CustomerId = Guid.Parse("7d6d4d39-7c9e-4f2e-a3f7-7f64540a4a57"),
        CoverageType = "Auto",
        RequestedAmount = 150000m,
        Currency = "USD"
    };

    public SubmitPolicyApplicationRequest SubmitLifePolicyApplication { get; init; } = new()
    {
        CustomerId = Guid.Parse("0e7e248f-b9f6-48d8-9228-6f4a9db6dd5f"),
        CoverageType = "Life",
        RequestedAmount = 240000m,
        Currency = "USD"
    };

    public SubmitPolicyApplicationRequest SubmitHomePolicyApplication { get; init; } = new()
    {
        CustomerId = Guid.Parse("cbf013ba-1b53-4f67-b05d-f74f8dbe73f9"),
        CoverageType = "Home",
        RequestedAmount = 350000m,
        Currency = "USD"
    };

    public Guid ExampleApplicationIdForGetOrCancel { get; init; } =
        Guid.Parse("d5e4ebf1-c4d1-4ff2-9898-f18d9964228a");

    public CancelPolicyApplicationRequest CancelApplicationRequest { get; init; } = new()
    {
        Reason = "Customer changed coverage requirements."
    };

    public Guid ExamplePolicyIdForGet { get; init; } =
        Guid.Parse("c9369774-8ec0-44ee-a09a-bf9fc84df284");
}
