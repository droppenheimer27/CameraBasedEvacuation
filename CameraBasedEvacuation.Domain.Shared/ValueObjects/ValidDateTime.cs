using System.Globalization;

namespace CameraBasedEvacuation.Shared.ValueObjects;

public readonly struct ValidDateTime : IValueObject<DateTime>
{
    private const string Iso8601Format = "yyyy-MM-ddTHH:mm:ssZ";
    
    public DateTime Value { get; }

    public ValidDateTime(DateTime value)
    {
        if (value == DateTime.MinValue)
        {
            throw new ArgumentException("Date cannot be the minimum DateTime value.", nameof(value));
        }

        if (value > DateTime.UtcNow)
        {
            throw new ArgumentException("Date cannot be in the future.", nameof(value));
        }

        Value = value;
    }

    public static implicit operator DateTime(ValidDateTime validDate) => validDate.Value;
    public static explicit operator ValidDateTime(DateTime value) => new(value);
    
    public override string ToString() => Value.ToString(Iso8601Format, CultureInfo.InvariantCulture);
}