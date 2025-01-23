using Akka.Actor;
using CameraBasedEvacuation.Shared.Events;
using CameraBasedEvacuation.Shared.ValueObjects;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RemoteCamera.Application.Settings;

namespace RemoteCamera.Application.Actors;

/// <summary>
/// Actor responsible for handling camera updates, processing incoming camera data, 
/// and forwarding it to the central server for further processing.
/// 
/// The actor listens for `CameraUpdateMessage` messages, converts them into a 
/// `CameraUpdated` event, and sends it to the central server actor for aggregation and analysis.
/// </summary>
public sealed class RemoteCamera : ReceiveActor
{
    private readonly IOptions<CentralServerActorSystemSettings> _centralServerSettings;
    private readonly RemoteCameraSettings _remoteCameraSettings;

    public RemoteCamera(IServiceProvider serviceProvider)
    {
        var scope = serviceProvider.CreateScope();
        
        _centralServerSettings = scope.ServiceProvider.GetRequiredService<IOptions<CentralServerActorSystemSettings>>();
        _remoteCameraSettings = scope.ServiceProvider.GetRequiredService<RemoteCameraSettings>();
        
        Receive<CameraUpdateMessage>(OnCameraUpdate);
    }
    
    private void OnCameraUpdate(CameraUpdateMessage message)
    {
        var monitor = Context.ActorSelection(_centralServerSettings.Value.ToActorPath());
        monitor.Tell(ConvertToEvent(message, _remoteCameraSettings.CameraId));
    }
    
    private static CameraUpdated ConvertToEvent(CameraUpdateMessage message, string cameraId)
        => new()
        {
            CameraId = new NotEmptyString(cameraId),
            Timestamp = message.Timestamp,
            In = message.In,
            Out = message.Out
        };
}