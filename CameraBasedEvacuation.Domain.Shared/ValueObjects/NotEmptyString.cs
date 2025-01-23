namespace CameraBasedEvacuation.Shared.ValueObjects;

public record NotEmptyString : IValueObject<string>
{
    public string Value { get; }

    public NotEmptyString(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Value cannot be null or whitespace.", nameof(value));
        }
        
        Value = value;
    }
    
    public static implicit operator string(NotEmptyString str) => str.Value;
    public static explicit operator NotEmptyString(string value) => new(value);
    
    public override string ToString() => Value;
}