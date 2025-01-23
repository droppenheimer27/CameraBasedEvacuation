using CameraBasedEvacuation.Shared.ValueObjects;
using MediatR;

namespace CentralServer.Application.CameraUpdates;

public record AddCameraUpdateCommand : IRequest
{
    public NotEmptyString CameraId { get; set; }
    
    public DateTime Timestamp { get; init; }
    
    public int In { get; init; }
    
    public int Out { get; init; }
}