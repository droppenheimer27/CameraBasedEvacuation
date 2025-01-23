using CentralServer.Domain.CameraUpdates;
using MediatR;

namespace CentralServer.Application.CameraUpdates;

public sealed class AddCameraUpdateHandler : IRequestHandler<AddCameraUpdateCommand>
{
    private readonly ICameraUpdatesRepository _cameraUpdates;

    public AddCameraUpdateHandler(ICameraUpdatesRepository cameraUpdates) 
        => _cameraUpdates = cameraUpdates;

    public Task Handle(AddCameraUpdateCommand command, CancellationToken cancellationToken)
    {
        _cameraUpdates.Save(ConvertToCameraUpdate(command));
        return Task.CompletedTask;
    }

    private static CameraUpdate ConvertToCameraUpdate(AddCameraUpdateCommand command)
        => new()
        {
            CameraId = command.CameraId,
            Timestamp = command.Timestamp,
            In = command.In,
            Out = command.Out
        };
}