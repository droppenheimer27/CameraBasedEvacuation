using CameraBasedEvacuation.Shared.ValueObjects;
using MediatR;

namespace CentralServer.Application.CameraUpdates;

public record GetPeopleCountOnCameraQuery : IRequest<PeopleCountDto>
{
    public NotEmptyString CameraId { get; init; }

    public GetPeopleCountOnCameraQuery(NotEmptyString cameraId)
    {
        CameraId = cameraId;
    }
}
