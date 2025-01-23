namespace CameraBasedEvacuation.Shared.ValueObjects;

public interface IValueObject<out TValue>
{
    TValue Value { get; }
}