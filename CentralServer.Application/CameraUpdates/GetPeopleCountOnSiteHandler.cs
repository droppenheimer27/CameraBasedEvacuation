using CentralServer.Domain.CameraUpdates;
using CentralServer.Domain.CameraUpdates.Exceptions;
using MediatR;

namespace CentralServer.Application.CameraUpdates;

public sealed class GetPeopleCountOnSiteHandler : IRequestHandler<GetPeopleCountOnSiteQuery, PeopleCountDto>
{
    private readonly ICameraUpdatesRepository _cameraUpdates;

    public GetPeopleCountOnSiteHandler(ICameraUpdatesRepository cameraUpdates) 
        => _cameraUpdates = cameraUpdates;

    public Task<PeopleCountDto> Handle(GetPeopleCountOnSiteQuery request, CancellationToken cancellationToken)
    {
        var updates = _cameraUpdates.GetAll();
        if (updates.Count == 0)
        {
            throw new CameraUpdateException("No available camera updates");
        }
        
        var latestTimestamp = updates.Max(update => update.Timestamp);
        var peopleOneSite = updates.CurrentCountPeopleOnSite(latestTimestamp);
        
        return Task.FromResult(new PeopleCountDto(peopleOneSite));
    }
}