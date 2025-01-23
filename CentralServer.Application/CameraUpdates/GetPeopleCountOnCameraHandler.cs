using CentralServer.Domain.CameraUpdates;
using CentralServer.Domain.CameraUpdates.Exceptions;
using MediatR;

namespace CentralServer.Application.CameraUpdates;

public sealed class GetPeopleCountOnCameraHandler : IRequestHandler<GetPeopleCountOnCameraQuery, PeopleCountDto>
{
    private readonly ICameraUpdatesRepository _cameraUpdates;

    public GetPeopleCountOnCameraHandler(ICameraUpdatesRepository cameraUpdates) => 
        _cameraUpdates = cameraUpdates;

    public Task<PeopleCountDto> Handle(GetPeopleCountOnCameraQuery request, CancellationToken cancellationToken)
    {
        var updates = _cameraUpdates.GetByCameraId(request.CameraId);
        if (updates.Count == 0)
        {
            throw new CameraUpdateException($"No available updates for camera '{request.CameraId}'");
        }
        
        var latestTimestamp = updates.Max(update => update.Timestamp);
        var peopleOneSite = updates.CurrentCountPeopleOnSite(latestTimestamp);

        return Task.FromResult(new PeopleCountDto(peopleOneSite));
    }
}