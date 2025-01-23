using CameraBasedEvacuation.Shared.ValueObjects;

namespace CameraBasedEvacuation.Shared.Events;

public sealed record CameraUpdated
{
    public NotEmptyString CameraId { get; init; }
    
    public ValidDateTime Timestamp { get; init; }
    
    public int In { get; init; }
    
    public int Out { get; init; }
}