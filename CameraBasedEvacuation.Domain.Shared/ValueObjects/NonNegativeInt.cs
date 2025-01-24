namespace CameraBasedEvacuation.Shared.ValueObjects;

public readonly struct NonNegativeInt : IValueObject<int>
{
    public int Value { get; }

    public NonNegativeInt(int value)
    {
        if (value < 0)
        {
            throw new ArgumentException("The value cannot be negative.", nameof(value));
        }

        Value = value;
    }

    public static implicit operator int(NonNegativeInt nonNegativeInt) => nonNegativeInt.Value;
    public static explicit operator NonNegativeInt(int value) => new(value);

    public override string ToString() => Value.ToString();
}