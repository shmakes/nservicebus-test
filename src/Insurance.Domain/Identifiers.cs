namespace Insurance.Domain;

public readonly record struct PolicyApplicationId(Guid Value)
{
    public static PolicyApplicationId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct PolicyId(Guid Value)
{
    public static PolicyId New() => new(Guid.NewGuid());
    public override string ToString() => Value.ToString();
}

public readonly record struct CustomerId(Guid Value)
{
    public override string ToString() => Value.ToString();
}
