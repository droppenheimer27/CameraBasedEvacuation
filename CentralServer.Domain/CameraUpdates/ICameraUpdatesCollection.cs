namespace CentralServer.Domain.CameraUpdates;

public interface ICameraUpdatesCollection : IReadOnlyCollection<CameraUpdate>
{
    int CurrentCountPeopleOnSite(DateTime latestTimestamp);
}