using CameraBasedEvacuation.Shared.ValueObjects;

namespace RemoteCamera.Application.Actors;

public record CameraUpdateMessage
{
    public ValidDateTime Timestamp { get; init; }
    
    public int In { get; init; }
    
    public int Out { get; init; }
}