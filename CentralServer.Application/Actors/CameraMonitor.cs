using System.Collections.Concurrent;
using Akka.Actor;
using CameraBasedEvacuation.Shared.Events;
using CentralServer.Application.CameraUpdates;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace CentralServer.Application.Actors;

/// <summary>
/// Actor responsible for processing camera updates in the central server.
/// 
/// This actor listens for `CameraUpdated` messages, which represent updates from remote cameras. Upon receiving
/// such messages, it processes them by logging relevant details and sending the updates to the central server's 
/// command handler. The actor also manages the lifecycle of camera actors, handling the `Terminated` 
/// message that is triggered when a monitored camera actor stops or disconnects, typically due to network issues 
/// or actor termination.
/// 
/// The `CameraMonitor` actor maintains a registry of active camera actors and ensures appropriate cleanup upon 
/// actor termination. The actor provides logging for monitoring and diagnostics during the process of handling 
/// camera updates and actor terminations.
/// </summary>
public sealed class CameraMonitor : ReceiveActor
{
    private readonly IServiceScope _scope;
    private readonly ILogger<CameraMonitor> _logger;
    private readonly IMediator _mediator;
    
    private readonly ConcurrentDictionary<IActorRef, string> _cameraActors = new();
    
    public CameraMonitor(IServiceProvider serviceProvider)
    {
        _scope = serviceProvider.CreateScope();
        _logger = _scope.ServiceProvider.GetRequiredService<ILogger<CameraMonitor>>();
        _mediator = _scope.ServiceProvider.GetRequiredService<IMediator>();
        
        ReceiveAsync<CameraUpdated>(OnCameraUpdate);
        
        // Handle Terminated messages for camera actors.
        // These messages are triggered when an actor being watched by Context.Watch dies.
        // Common causes include network issues, deliberate termination, or unexpected failures.
        // If the terminated actor was tracked, log and remove it from the camera registry.
        // If not, log a warning to highlight the anomaly.
        Receive<Terminated>(OnTermination);
    }
    
    private async Task OnCameraUpdate(CameraUpdated message)
    {
        try
        {
            // Notify this actor when the sender (camera actor) stops or disconnects.
            // Context.Watch ensures we monitor the lifecycle of camera actors. 
            // If a camera actor stops due to disconnection, network failure, or manual termination,
            // a Terminated message will be sent to this actor, allowing cleanup or logging actions.
        
            Context.Watch(Sender);
            _cameraActors.TryAdd(Sender, message.CameraId);
        
            _logger.LogInformation(
                "Received update from camera: '{cameraId}' at '{timestamp}'",
                message.CameraId,
                message.Timestamp
            );

            await _mediator.Send(ConvertToCommand(message));
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Error occured during processing '{message}': '{error}'",
                nameof(CameraUpdated),
                exception.Message
            );
            
            throw;
        }
    }

    private void OnTermination(Terminated message)
    {
        var removed = _cameraActors.TryRemove(message.ActorRef, out var cameraId);
        if (removed)
        {
            _logger.LogWarning(
                "Camera '{cameraId}' was disconnected. Actor Path: '{actorRef}'",
                cameraId,
                message.ActorRef
            );
        }
    }
    
    protected override void PreStart()
    {
        _logger.LogInformation("Starting actor: {actorName}", nameof(CameraMonitor));
    }
    
    protected override void PostStop()
    {
        _logger.LogInformation("Stopping actor: {actorName}", nameof(CameraMonitor));
        _scope.Dispose();
    }

    private static AddCameraUpdateCommand ConvertToCommand(CameraUpdated message) => 
        new()
        {
            CameraId = message.CameraId,
            Timestamp = message.Timestamp,
            In = message.In,
            Out = message.Out
        };
}