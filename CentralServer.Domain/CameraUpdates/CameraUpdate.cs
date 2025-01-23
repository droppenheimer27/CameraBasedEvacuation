using CameraBasedEvacuation.Shared.ValueObjects;
using CentralServer.Domain.Shared;

namespace CentralServer.Domain.CameraUpdates;

public class CameraUpdate : IEntity
{
    public Guid Id { get; set; }
    
    public NotEmptyString CameraId { get; set; }
    
    public DateTime Timestamp { get; set; }
    
    public int In { get; set; }
    
    public int Out { get; set; }

    public CameraUpdate()
    {
        Id = Guid.NewGuid();
    }
}