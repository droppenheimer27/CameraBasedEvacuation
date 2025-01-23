using CameraBasedEvacuation.Shared.ValueObjects;

namespace CentralServer.Domain.CameraUpdates;

public interface ICameraUpdatesRepository
{
    void Save(CameraUpdate update);
    
    ICameraUpdatesCollection GetByCameraId(NotEmptyString cameraId);
    
    ICameraUpdatesCollection GetAll();
}