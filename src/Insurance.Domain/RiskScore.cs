namespace Insurance.Domain;

public readonly record struct RiskScore
{
    public int Value { get; }

    public RiskScore(int value)
    {
        if (value is < 0 or > 100)
        {
            throw new ArgumentOutOfRangeException(nameof(value), "Risk score must be between 0 and 100.");
        }

        Value = value;
    }

    public override string ToString() => Value.ToString();
}
