using CameraBasedEvacuation.Shared.ValueObjects;
using CentralServer.Domain.CameraUpdates;

namespace CentralServer.Infrastructure.Persistence.CameraUpdates;

public sealed class CameraUpdatesRepository : ICameraUpdatesRepository
{
    private readonly IPersistence<CameraUpdate> _persistence;

    public CameraUpdatesRepository(IPersistence<CameraUpdate> persistence)
    {
        _persistence = persistence;
    }

    public void Save(CameraUpdate cameraUpdate) 
        => _persistence.Save(cameraUpdate);

    public ICameraUpdatesCollection GetByCameraId(NotEmptyString cameraId)
        => new CameraUpdatesCollection(_persistence.GetAll().Where(update => update.CameraId == cameraId));

    public ICameraUpdatesCollection GetAll()
        => new CameraUpdatesCollection(_persistence.GetAll());
}